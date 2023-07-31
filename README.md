# **GOAP**

## Introduction

En este trabajo, presentamos una implementación básica del algoritmo Goal Oriented Action Planning (GOAP) aplicado en un escenario interactivo compuesto por cuatro agentes distintos: una persona (blanco), un sicario (rojo), un detective (azul) y un objetivo o target (verde). Estos agentes interactúan en un entorno en el cual se distribuyen varios ítems, desempeñando un papel crucial en el funcionamiento del algoritmo, ya que cada acción se vincula con un estado e ítem específicos.

La implementación del GOAP en este contexto permite a los agentos tomar decisiones basadas en sus objetivos y el estado del mundo que les rodea, brindando un comportamiento dinámico y adaptativo a cada uno de ellos.

Además del GOAP, el proyecto incorpora otras técnicas importantes de la programación de videojuegos. Una de ellas es la implementación de una máquina de estados, la cual delega toda su ejecución a otra clase, permitiendo la reutilización del código en diversas situaciones. También se utiliza una grilla para optimizar la búsqueda y el pathfinding, para lo cual hemos utilizado la herramienta Thinking Agent que emplea LINQ en una clase estática para la búsqueda de ítems en la escena.

En cuanto al código, se incluyen extensiones para el debugging de ejecuciones lazy y la implementación de observables para disparar eventos específicos con la ayuda de los delegados. Estas herramientas nos permiten mantener un seguimiento detallado del funcionamiento del algoritmo y del comportamiento de los agentes, así como detectar y corregir posibles errores o ineficiencias en la ejecución.

Este informe proporcionará una descripción detallada de cómo se han implementado estas características y cómo interactúan entre sí para dar lugar a un juego dinámico y atractivo.


## Diseño e Implementación de GOAP
El diseño e implementación del algoritmo de Goal Oriented Action Planning (GOAP) fue un proceso meticuloso y detallado que requería comprensión y manipulación efectiva de los conceptos de planificación de acciones y pathfinding.

Inicié el proceso con la elaboración de dos ejemplos prácticos, los cuales fueron diseñados con el objetivo de descomponer y entender en detalle la ejecución del algoritmo GOAP. Posteriormente, me dediqué a estudiar en profundidad los algoritmos de pathfinding, basándome en el artículo disponible en Red Blob Games. Esto me permitió obtener una comprensión sólida del funcionamiento de estos algoritmos y prepararme para modificar el algoritmo base de GOAP de uno que trabaja con valores booleanos a uno que permite distintos tipos de datos.

Los diseños de estos ejemplos están disponibles en la misma carpeta del proyecto y llevan los nombres: [GOAP - GOAP Example 1](/primer_archivo.pdf), [GOAP - GOAP Example 2](/primer_archivo.pdf)y [GOAP - GOAP Example 3](/primer_archivo.pdf). Las ejecuciones de estos ejemplos se pueden encontrar en el script GoapMiniTest.cs, el cual fue utilizado como espacio de prueba o 'sandbox' para llevar a cabo estos tres casos.

A partir del análisis del código y los experimentos realizados, decidí que la mejor manera de implementar el algoritmo era utilizar objetos y delegados, con acciones y funciones que asignan los valores correspondientes a constantes en el diccionario. Para soportar esto, creé nuevas clases llamadas GOAPStateDelegate.cs y GOAPActionDelegate.cs.

Además, introduje un nuevo atributo llamado caseFromValues de tipo Dictionary<string, Func<GOAPStateDelegate, bool>>. Esta estructura de datos permitió evaluar, por medio de lambdas, si se había alcanzado la meta del plan, facilitando así la evaluación de las condiciones y el seguimiento del progreso del agente hacia su objetivo.


## Pathfinding A* & GOAP

En esta implementación, se emplea una versión lazy del algoritmo A* para planificar el camino que seguirán los agentes en el juego.

El código a continuación muestra cómo se ha implementado el algoritmo A*:
```
public IEnumerator GoapRunDelegate(
    GOAPStateDelegate from,
    GOAPStateDelegate to,
    IEnumerable<GOAPActionDelegate> actions
) {
    Func<GOAPStateDelegate, GOAPStateDelegate, float> heuristic = (curr, goal) => {
        return goal.caseFromValues.Count(goalKv => !goalKv.Value(curr));
    };
    
    yield return AStarTimeSlice<GOAPStateDelegate>.Run(
        from,
        to,
        heuristic,
        curr => {
            return to.caseFromValues.All(kv => kv.Value(curr));
        },
        curr => {
            if (securityStopWatch == 0) {
                return Enumerable.Empty<AStarTimeSlice<GOAPStateDelegate>.Arc>();
            }

            securityStopWatch--;

            return actions
                .Where(action => action.ValidatePreconditions(curr))
                .Aggregate(new FList<AStarTimeSlice<GOAPStateDelegate>.Arc>(), (possibleList, action) => {
                    var newState = new GOAPStateDelegate(curr);
                    newState.ApplyEffects(action.effects);
                    newState.generatingAction = action;
                    newState.step = curr.step + 1;
                    return possibleList + new AStarTimeSlice<GOAPStateDelegate>.Arc(newState, action.cost);
                });
        });
    var seq = AStarTimeSlice<GOAPStateDelegate>.GetSequence();

    Debug.LogColor("PLAN", "green");
    Debug.LogColor("watchdog: " + securityStopWatch, "green");
    if (seq == null) {
        Debug.LogError("impossible to plan");
        yield break;
    }
    var costSoFar = 0f;
    foreach (var act in seq.Skip(1)) {
        var heuristicValue = heuristic(act, to);
        var dijkstraValue = act.generatingAction.cost;
        costSoFar += dijkstraValue + heuristicValue;
        var stringValues = "\n" + "(CostSoFar: " + costSoFar + ")" + "\n" +
                           "(Heuristic: " + heuristicValue + ")" + "\n" +
                           "(Dijkstra: " + dijkstraValue + ")" + "\n" +
                           "(Step: " + act.step + ")";
        Debug.Log(act + "----------------------------" + stringValues);
    }
    this.plan = seq.Skip(1).Select(x => x.generatingAction);
}
```
En este código, se define una función heurística que compara el estado actual del agente `curr` con el estado objetivo `goal`. Luego, se inicia la ejecución del algoritmo A* utilizando la clase AStarTimeSlice, que permite ejecutar el algoritmo de forma **"lazy"**, permitiendo una ejecución más eficiente y evitando la necesidad de calcular todo el camino de antemano.

Las acciones posibles en cada estado se filtran a través de sus precondiciones y se agrega al estado actual los efectos de cada acción validada. Luego, cada nuevo estado posible (junto con su costo asociado) se agrega a una lista de posibles arcos para la próxima iteración del algoritmo.

Una vez que se ha encontrado un camino, se registra en el plan de este agente para su posterior ejecución. Si no se encuentra un camino, se muestra un error, indicando que es imposible planificar.

## AStarTimeSlice

```
    public static IEnumerator Run
    (
        Node from,
        Node to,
        // Heuristic cost
        Func<Node, Node, float> heuristic,
        // Satisfies
        Func<Node, bool> satisfies,
        // Endpoints costs
        Func<Node, IEnumerable<Arc>> expand
    ) {
        var initialState = new AStarState<Node>();
        initialState.open.Add(from);
        initialState.gs[from] = 0;
        initialState.fs[from] = heuristic(from, to);
        initialState.previous[from] = null;
        initialState.current = from;

        var state = initialState;
        while (state.open.Count > 0 && !state.finished) {
            // Debugger gets buggy af with this, can't watch variable:
            state = state.Clone();

            var candidate = state.open.OrderBy(x => state.fs[x]).First();
            state.current = candidate;

            UpdateUIGoap.actionsCounter[candidate.ToString()] = 
                UpdateUIGoap.actionsCounter.DefaultGet(candidate.ToString(), () => 0) + 1;
            
            if (satisfies(candidate)) {
                state.finished = true; // Has been found the goal!!!
            }

            state.open.Remove(candidate);
            state.closed.Add(candidate);
            _timeSlice--;
            if (_timeSlice <= 0) {
                _timeSlice = 10;
                yield return new WaitForEndOfFrame();
            }
            var neighbours = expand(candidate);

            // has neighbours ?
            if (neighbours == null || !neighbours.Any()) continue;

            var gCandidate = state.gs[candidate];

            foreach (var ne in neighbours) {
                if (ne.endpoint.In(state.closed)) continue;

                var gNeighbour = gCandidate + ne.cost;
                state.open.Add(ne.endpoint);

                if (gNeighbour > state.gs.DefaultGet(ne.endpoint, () => gNeighbour)) continue;

                state.previous[ne.endpoint] = candidate;
                state.gs[ne.endpoint] = gNeighbour;
                state.fs[ne.endpoint] = gNeighbour + heuristic(ne.endpoint, to);
            }
        }
```

La función Run es la implementación central del algoritmo A* en esta solución. A continuación se detallan los componentes de esta función.


La función toma cinco argumentos:

1. from: nodo de inicio para el pathfinding.
2. to: nodo destino del pathfinding.
3. heuristic: función que toma dos nodos y devuelve un flotante que representa el costo heurístico entre los nodos.
4. satisfies: función que toma un nodo y verifica si cumple con las condiciones necesarias para ser el nodo objetivo.
5. expand: función que toma un nodo y devuelve una lista de arcos que representan los nodos vecinos a los que se puede llegar desde el nodo actual.
La función comienza inicializando el estado de A* y luego entra en un bucle donde sigue buscando el camino mientras haya nodos abiertos y no se haya alcanzado el nodo objetivo.

Dentro del bucle, se selecciona el nodo con el costo total más bajo (costo de camino más costo heurístico) y se verifica si cumple con las condiciones para ser el nodo objetivo.

Luego, se actualiza el estado quitando el nodo candidato de la lista abierta y agregándolo a la lista cerrada. En este punto, se verifica si se ha agotado el límite de tiempo asignado para cada cuadro *_timeSlice*, y si es así, se cede el control para permitir que otros procesos se ejecuten.

Posteriormente, se expande el nodo candidato para obtener los nodos vecinos a los que se puede llegar desde él. Si no hay vecinos, el bucle continúa con el siguiente nodo.

Finalmente, para cada vecino, se verifica si ya se ha evaluado. Si no es así, se calcula el nuevo costo total para llegar a ese nodo (gNeighbour) y se actualiza el estado si este costo es menor que el costo almacenado actualmente para ese nodo.

Este proceso continúa hasta que se ha encontrado el camino al nodo objetivo o hasta que se han evaluado todos los nodos posibles. Este tipo de implementación basada en generadores permite que la búsqueda se realice de forma **"lazzy"**, lo que puede mejorar el rendimiento, especialmente en escenas de juego grandes o complejas.

## Conclusiones

A lo largo del desarrollo de este proyecto, he podido adquirir un profundo entendimiento del algoritmo Goal Oriented Action Planning (GOAP) y su aplicación en un entorno de juego. He dedicado un tiempo considerable a desentrañar los mecanismos subyacentes del algoritmo, a través de la creación de varios ejemplos que me permitieron observar su comportamiento en diferentes situaciones. Este proceso de aprendizaje me ha permitido adaptar y aplicar el algoritmo GOAP de una manera efectiva y eficiente en mi juego.

La implementación del pathfinding utilizando A* funcional lazy representó un desafío interesante. Me basé en el artículo de Red Blob Games para entender los matices del algoritmo y cómo implementarlo de manera efectiva. Este enfoque permitió que los agentes en el juego pudieran moverse de manera eficiente, seleccionando siempre el camino de menor costo.

Además, la adaptación del modelo de mundo de un simple diccionario de booleanos a un modelo que permite múltiples tipos de datos (flotante, entero, booleano, cadena) fue un aspecto crucial de este proyecto. Esto no solo permitió una mayor flexibilidad en el diseño del juego, sino que también enriqueció la interacción entre los agentos y el mundo del juego.

En cuanto a la implementación de la máquina de estados, estoy satisfecho con la decisión de delegar su ejecución a otra clase. Esto permitió que las funciones de la máquina de estados fueran reutilizables, lo que facilitó la implementación y la mantenibilidad del código.

Finalmente, también considero que la creación de extensiones para el depurador y la implementación de observables para disparar eventos específicos fueron importantes para el éxito del proyecto. Estas herramientas me permitieron seguir y entender el flujo del programa, y facilitaron enormemente el proceso de depuración.

En resumen, este proyecto me ha permitido profundizar mis habilidades de programación y comprender en profundidad el algoritmo GOAP y su aplicación en los videojuegos. Estoy satisfecho con el trabajo realizado y creo que las habilidades y conocimientos adquiridos serán de gran valor en mis futuros proyectos.