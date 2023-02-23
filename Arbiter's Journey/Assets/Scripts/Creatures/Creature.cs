using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [SerializeField]
    private Vector2 gridPosition;
    [SerializeField]
    private Sprite sprite;

    void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddCreatureAtGridPosition(gridPosition, this);
    }

    void Update()
    {
        
    }
}
