using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridDebugObject : MonoBehaviour
{
    private GridObject gridObject;

    [SerializeField]
    private TextMeshPro text;

    private void Start()
    {
        if (text == null)
        {
            text = GetComponentInChildren<TextMeshPro>();
        }
    }

    public void SetGridObject(GridObject gridObject)
    {
        Debug.Log("Called SetGridObject at " + gridObject.ToString());
        this.gridObject = gridObject;
        UpdateText(gridObject);
    }

    private void UpdateText(GridObject gridObject)
    {
        text.text = gridObject.ToString();
    }
}
