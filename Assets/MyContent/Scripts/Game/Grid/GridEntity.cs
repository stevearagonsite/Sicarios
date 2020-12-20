using System;
using UnityEngine;

[ExecuteInEditMode]
public class GridEntity : MonoBehaviour {

	public event Action<GridEntity> OnMove = delegate {};
	public Grid grid { get; private set; }
    public static GridEntity instance;

    void Start() {
        grid = Grid.instance;
		Debug.Assert(grid != null, "Should be a child of a Grid");
		grid.Track(this);
	}

    public void Update()
    {
        Execute();
    }

    public void Execute()
    {
        OnMove(this);
    }

	void OnDestroy() {
		grid.Untrack(this);
	}
}
