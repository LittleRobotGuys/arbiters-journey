using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingGrid : MonoBehaviour
{
    [SerializeField]
    private Transform gridDebugObjectPrefab;
    private GridSystem gridSystem;

    void Start()
    {
        gridSystem = new GridSystem(5, 7, .25f);

        if (gridSystem == null)
        {
            Debug.Log("WHY IS THERE NO GRID SYSTEM?!?!?");
        }
        else
        {
            Debug.Log("Made a grid system!" + gridSystem.ToString());
        }

        if (gridDebugObjectPrefab != null)
        {
            Debug.Log("Grid system: " + gridSystem.ToString() + "\ngridDebugObjectPrefab: " + gridDebugObjectPrefab.ToString());
            gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
        }
        else 
        {
            Debug.Log("Grid system was not created!");
        }
    }
}
