using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using U = Utility;

[ExecuteInEditMode]
public class Grid : MonoBehaviour {
	public float x;
	public float z;
	public float cellWidth;
	public float cellHeight;
	public int width;
	public int height;

    public static Grid instance;

    readonly public Tuple<int, int> Outside = Tuple.Create(-1, -1);

	Dictionary<GridEntity, Tuple<int, int>> _lastPositions;
	HashSet<GridEntity>[,] _buckets;

	public IEnumerable<GridEntity> Query(Vector3 aabbFrom, Vector3 aabbTo, Func<Vector3, bool> filterByPosition) {
		//Si hace falta damos vuelta para que "from" quede siempre en la punta inferior (ej: arriba-izq) y "to" en la punta superior (ej: abajo-der)
		var from = new Vector3(Mathf.Min(aabbFrom.x, aabbTo.x), 0, Mathf.Min(aabbFrom.z, aabbTo.z));
		var to  = new Vector3(Mathf.Max(aabbFrom.x, aabbTo.x), 0, Mathf.Max(aabbFrom.z, aabbTo.z));

		//Calculamos la coordenada entera (para la grilla de buckets)
		var fromCoord = PositionInGrid(from);
		var toCoord = PositionInGrid(to);

		//Ajustamos (clamp) a coordenadas válidas
        fromCoord = Tuple.Create(Mathf.Clamp(fromCoord.Item1, 0, width), Mathf.Clamp(fromCoord.Item2, 0, height));
        toCoord = Tuple.Create(Mathf.Clamp(toCoord.Item1, 0, width), Mathf.Clamp(toCoord.Item2, 0, height));
		
		// Creamos tuplas de cada celda que toca el AABB
		var cols = U.Generate(fromCoord.Item1, x => x + 1)				
			.TakeWhile(x => x <= toCoord.Item1);

		var rows = U.Generate(fromCoord.Item2, y => y + 1)
			.TakeWhile(y => y <= toCoord.Item2);

		var cells = cols.SelectMany(
            col => rows.Select(
                row => Tuple.Create(col, row)
            )
        );

		// Levantamos todas las entidades de los buckets que toca, filtramos por AABB, luego filtramos por predicado personalizado (filterByPosition)
		return cells
            .SelectMany(cell => _buckets[cell.Item1, cell.Item2])
			.Where(e =>
				from.x <= e.transform.position.x && e.transform.position.x <= to.x &&
				from.z <= e.transform.position.z && e.transform.position.z <= to.z
			).Where(x => filterByPosition(x.transform.position));
	}

	void UpdateEntity(GridEntity entity) {
		//Averiguamos y calculamos la posición previa y la nueva respectivamente
		var prevPos = _lastPositions.ContainsKey(entity) ? _lastPositions[entity] : Outside;
		var currPos = PositionInGrid(entity.transform.position);

		//Misma posición, no hace falta actualizar nada (optimización)
		if(prevPos.Equals(currPos))
			return;

		//La entidad estaba previamente dentro de la grilla, así que la sacamos del bucket en el que estaba.
		if(InsideGrid(prevPos)) {
			_buckets[prevPos.Item1, prevPos.Item2].Remove(entity);
		} 

		//Si la entidad esta ahora dentro de la grilla, la movemos al bucket correspondiente, si esta fuera dejamos de recordar su ultima posición.
		if(InsideGrid(currPos)) {
			_buckets[currPos.Item1, currPos.Item2].Add(entity);
			_lastPositions[entity] = currPos;
		} 
		else {
			_lastPositions.Remove(entity);
		}
	}


	//¿Está la COORDENADA ENTERA dentro de la grilla de buckets?
	bool InsideGrid(Tuple<int, int> position) {
		return 0 <= position.Item1 && position.Item1 < width &&
			0 <= position.Item2 && position.Item2 < height;
	}

	//Conversión de posición del mundo a COORDENADA ENTERA de la grilla de buckets
    Tuple<int, int> PositionInGrid(Vector3 position)
    {
        return Tuple.Create(
            (int)Mathf.Floor((position.x - x) / cellWidth),
            (int)Mathf.Floor((position.z - z) / cellHeight)
        );
    }

	public void Track(GridEntity entity) {
		entity.OnMove += UpdateEntity;
		UpdateEntity(entity);	//En caso que nunca se mueva, actualizemosla una vez
	}

	public void Untrack(GridEntity entity) {
		entity.OnMove -= UpdateEntity;	
		
		//Eliminamos referencias colgantes	
		Tuple<int, int> coords;
		if(_lastPositions.TryGetValue(entity, out coords))
			_buckets[coords.Item1, coords.Item2].Remove(entity);
	}

	void Awake() {

        if (instance != null)
        {
            instance = null;
            instance = this;
        }
        else
        {
            instance = this;
        }

        _lastPositions = new Dictionary<GridEntity, Tuple<int, int>>();
		_buckets = new HashSet<GridEntity>[width, height];

		for (int i = 0;  i < width; i++)
			for (int j = 0;  j < height; j++)
				_buckets[i, j] = new HashSet<GridEntity>();
	}


	void OnDrawGizmos() {
		var cols = U.Generate(x, curr => curr + cellWidth)
			.Select(col => Tuple.Create(
                new Vector3(col, 0, z),
                new Vector3(col, 0, z + cellHeight * height)
                )
        );

		var rows = U.Generate(z, curr => curr + cellHeight)
			.Select(row => Tuple.Create(
                new Vector3(x, 0, row),
                new Vector3(x + cellWidth * width, 0, row)
                )
            );

        var allLines =
            cols
                .Take(width + 1)
                .Concat(
                    rows.Take(height + 1)
                    );

        foreach (var line in allLines)
			Gizmos.DrawLine(line.Item1, line.Item2);

		if(_buckets != null) {
			var toDraw = U.LazyMatrix(_buckets)
                .Where(t => t.Item3.Count > 0)
                .ToList();

			//Flatten the sphere we're going to draw
			Gizmos.matrix *= Matrix4x4.Scale(Vector3.forward + Vector3.right);
			foreach(var t in toDraw) {
				var ix = t.Item1;
				var iy = t.Item2;
				Gizmos.color = Color.blue;
				Gizmos.DrawWireSphere(
					new Vector3(x, 0, z) +
					new Vector3(
                        (ix + 0.5f) * cellWidth,
                        0,
                        (iy + 0.5f) * cellHeight),
					Mathf.Min(cellWidth, cellHeight) / 2
				);
			}

			Gizmos.color = Color.red;
			Gizmos.matrix = Matrix4x4.identity;
			foreach(var t in toDraw.Where(t => t.Item3.Count >= 2)) {
				var pairs =
                    t.Item3.SelectMany(
                        e1 => t.Item3.Select(
                            e2 => Tuple.Create(e1, e2)
                        )
                    )
                    .Where(pair => pair.Item1 != pair.Item2)
                    ;

				var offset = Vector3.up * 3.0f;
				foreach(var te in pairs) {
					Gizmos.DrawLine(te.Item1.transform.position+offset, te.Item2.transform.position+offset);
					Gizmos.DrawCube(te.Item1.transform.position+offset, Vector3.one);
				}			
			}
		}
	}

	
}
