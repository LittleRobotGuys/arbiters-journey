using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class Creature : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer sprite;
    [SerializeField]
    private float moveSpeed = .25f;
    [SerializeField]
    private bool selected = false;
    [SerializeField]
    private CreatureAnimator animator = null;

    [SerializeField]
    private Pathfinding pathfinding;

    private SmartTile creatureTile;
    private List<PathNode> path;

    [SerializeField]
    private SmartTileMap groundTileMap;
    [SerializeField] 
    private SmartTileMap collisionTileMap;
    private bool isMoving = false;
    private Vector3 IMPOSSIBLE_V3 = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

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

        if (creatureTile == null)
        {
            UpdateCreatureTile();
        }

        if (pathfinding == null)
        {
            Debug.LogWarning("Pathfinding was NULL for some reason.");
            pathfinding = ScriptableObject.CreateInstance<Pathfinding>();
        }

        gridManager.LazyAddCreature(this);

        pathfinding.SetTilemap(groundTileMap);

    }

    private void UpdateCreatureTile()
    {
        creatureTile = groundTileMap.GetTile(new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z));
    }


    void Update()
    {
        if (creatureTile == null)
        {
            creatureTile = groundTileMap.GetTile(transform.position.x, transform.position.y);
        }
    }

    public void MouseClicked(SmartTile tile)
    {
        if (tile != null && this.selected)
        {
            Debug.Log(name + " to " + tile.ToString() + " from " + this.creatureTile.ToString());
            if (tile.ToString() != this.creatureTile.ToString())
            {
                if (isMoving) return;
                path = pathfinding.FindPath(this.creatureTile, tile);
                // animator.Animate(path[0].GetLocationAsV3(), 5000);
                if (path != null) StartCoroutine(MoveToTile());
                else Debug.LogWarning("Something happened when trying to get to " + tile.ToString() + " from " + creatureTile.ToString());  
            }
            else Debug.Log(name + " cannot move to tile, already there.");
        }
    }


    bool readyForNextTile = true;
    private IEnumerator MoveToTile()
    {
        isMoving = true;
        readyForNextTile = true;

        Vector3 NextTile = this.IMPOSSIBLE_V3;
        Debug.Log("PATH COUNT " + path.Count);
        for (int t = 1; t < path.Count;)
        {
            NextTile = path[t].GetLocationAsV3();

            if (readyForNextTile)
            {
                TweenAlongPath(NextTile);
                UpdateCreatureTile();
                t++;
            }

            yield return new WaitForEndOfFrame();
        }

        // Finalization of placement ensures Character ends up where they should be even in edge cases.
        transform.position = NextTile;
        UpdateCreatureTile();
        animator.ResetAnimation();

        path = null;
        isMoving = false;
    }
    private void TweenAlongPath(Vector3 path)
    {
        // DOMove(end,duration, snapping)
        readyForNextTile = false;
        animator.Animate(path - transform.position, moveSpeed);
        transform.DOMove(path, moveSpeed, true).OnComplete(MoveOn);
        animator.StopAnimating();
    }

    private void MoveOn()
    {
        readyForNextTile = true;
    }


    public Pathfinding GetPathfinding()
    {
        return this.pathfinding;
    }

    internal SmartTile GetTile()
    {
        return this.creatureTile;
    }

}
