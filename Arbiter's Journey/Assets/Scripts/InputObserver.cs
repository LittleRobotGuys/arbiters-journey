using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputObserver : MonoBehaviour
{
    [SerializeField]
    private LayerMask terrainMask;
    [SerializeField]
    private LayerMask characterMask;

    private static InputObserver instance;

    private void Awake()
    {
        instance = this;
    }


    private static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.Log(Physics.Raycast(ray, out RaycastHit raycastHit, instance.terrainMask));
        return raycastHit.point;
    }
}
