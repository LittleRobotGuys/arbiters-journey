using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Creature : MonoBehaviour
{
    [SerializeField]
    private Vector2Int gridPosition;
    [SerializeField]
    private SpriteRenderer sprite;
    [SerializeField]
    private float moveSpeed = 0.04f;
    [SerializeField]
    private bool selected = false;
    [SerializeField]
    private CreatureAnimator animator = null;

    [SerializeField]
    private bool DebuggingMovement = false;
    [SerializeField]
    private Vector2Int DebugMoveTarget = new Vector2Int(5, 3);

    private Tile creatureTile;

    private Vector2Int targetPosition;
    private float stopDistance = 0.4f;
    private Tilemap groundTileMap;
    private Tilemap collisionTileMap;

    private void Awake()
    {
        if (sprite == null)
        {
            sprite = GetComponentInChildren<SpriteRenderer>();
        }

        if (animator == null)
        {
            animator = gameObject.GetComponent<CreatureAnimator>();
        }
    }

    void Start()
    {
        GridManager gridManager = GameObject.FindObjectOfType<GridManager>();
        if(gridManager == null)
        {
            Debug.LogError("NO GRID MANAGER FOUND IN SCENE!");
        }
        if (groundTileMap == null)
        {
            groundTileMap = gridManager.GetWalkableTileMap();
        }
        if(collisionTileMap == null)
        {
            collisionTileMap = gridManager.GetCollisionTileMap();
        }

        gridPosition = new Vector2Int(0, 0);

        if (DebuggingMovement)
        {
            Move(DebugMoveTarget);
        }

        if (creatureTile == null)
        {
            GetTile(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        }
    }

    /**
     * Dictates movement of a character with an assumption of using the 16x16 pixel movement, though currently
     * there is nothing that requires a specific pixel amount or size.
     * 
     * Move is called by Start in debug characters, and will be called by events responding to Input as well.
     * 
     * this.targetPosition is continually moved towards in the Update() method.
     */
    public void Move(Vector2Int targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    void Update()
    {
        Vector2Int transformV2i = new Vector2Int((int)transform.position.x, (int)transform.position.y);

        Tile targetTile = GetTile(targetPosition);
        
        if (creatureTile.transform.GetPosition() != targetTile.transform.GetPosition())
        {
            Vector2 moveDirection = (targetPosition - transformV2i);
            float t = Time.deltaTime;

            Vector3 moveVector = (moveDirection * moveSpeed * t).normalized;
            Vector3 moveVectorSecondary;
            if (Math.Abs((int)moveVector.x) > Math.Abs((int)moveVector.y) && ((int)moveVector.x != 0 || (int) moveVector.y != 0))
            {
                moveVector = moveVector.x < 0 ? new Vector3(-1, 0, 0) : new Vector3(1, 0, 0);
                moveVectorSecondary = moveVector.y < 0 ? new Vector3(0, -1, 0) : new Vector3(0, 1, 0);
            }
            else
            {
                moveVector = moveVector.y < 0 ? new Vector3(0, -1, 0) : new Vector3(0, 1, 0);
                moveVectorSecondary = moveVector.x < 0 ? new Vector3(-1, 0, 0) : new Vector3(1, 0, 0);
            }

            if (!CheckForObstacles(moveVector))
            {
                
                transform.position += moveVector;
                this.animator.Animate(moveDirection, t / moveSpeed);
            }
            else if (!CheckForObstacles(moveVectorSecondary))
            {
                transform.position += moveVectorSecondary;
                this.animator.Animate(moveDirection, t / moveSpeed);
            }
            else
            {
                this.targetPosition = transformV2i;
            }
        }
        else
        {
            this.animator.StopAnimating();
        }

    }

    private Tile GetTile(Vector2Int targetPosition)
    {
        Tile tile = new Tile();

            

        return tile;
    }

    private bool CheckForObstacles(Vector3 moveVector)
    {
        Vector3Int testPosition = groundTileMap.WorldToCell(transform.position + (Vector3)(transform.position + moveVector));
        if(collisionTileMap.HasTile(testPosition))
        {
            return true;
        }
        return false;
    }
}
