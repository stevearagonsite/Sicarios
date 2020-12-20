using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour{
    public GameObject objectToSpawn;
    public KeyCode key;
    

    //TODO: Delete this class for build
    //Only for debug the grid and game
    void Update() {
        if (!Input.GetKeyDown(key)) return;
        var grid = GetComponent<Grid>();
        
        for (int i = 0; i < 100; i++) {
            var gameObject = Instantiate(objectToSpawn, transform, true);
            gameObject.transform.position = new Vector3(
                Random.Range(grid.x, grid.x + grid.width * grid.cellWidth)
                , 0
                , Random.Range(grid.z, grid.z + grid.height * grid.cellHeight)
            );
        }
    }
}