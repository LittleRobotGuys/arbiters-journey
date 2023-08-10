using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class Creature : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer sprite;
    [SerializeField]
    private float moveSpeed = 0.04f;
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
                animator.Animate(path[0].GetLocationAsV3(), 5000);
                if (path != null) StartCoroutine(MoveToTile());
                else Debug.LogWarning("Something happened when trying to get to " + tile.ToString() + " from " + creatureTile.ToString());  
            }
            else Debug.Log(name + " cannot move to tile, already there.");
        }
    }

    private IEnumerator MoveToTile()
    {
        isMoving = true;
        bool readyForNextTile = false;
        Vector3 NextTile = this.IMPOSSIBLE_V3;
        Debug.Log("PATH COUNT " + path.Count);
        for (int t = 1; t < path.Count;)
        {
            NextTile = path[t].GetLocationAsV3();

            if (!readyForNextTile)
            {
                readyForNextTile = JumpAlongPath(NextTile);
                UpdateCreatureTile();
                t++;
            }

            yield return new WaitForEndOfFrame();
        }

        // Finalization of placement ensures Character ends up where they should be even in edge cases.
        transform.position = NextTile;
        UpdateCreatureTile();
        animator.StopAnimating();

        path = null;
        isMoving = false;
    }


    private IEnumerator MoveToTileWorking()
    {
        isMoving = true;
        float stepCount = 1;
        float maxStep = 40;
        Vector3 NextTile = this.IMPOSSIBLE_V3;
        Debug.Log("PATH COUNT " + path.Count);
        for (int t = 1; t < path.Count; t++)
        {
            NextTile = path[t].GetLocationAsV3();

            if (stepCount <= maxStep)
            {
                JumpAlongPath(NextTile);
                stepCount++;
            }
            else
            {
                stepCount = 1;
                UpdateCreatureTile();
            }

            t = stepCount < maxStep ? t - 1 : t;

            yield return new WaitForFixedUpdate();
        }

        // Finalization of placement ensures Character ends up where they should be even in edge cases.
        transform.position = NextTile;
        UpdateCreatureTile();
        animator.StopAnimating();

        path = null;
        isMoving = false;
    }


    private bool JumpAlongPath(Vector3 nextTile)
    {
        // Readability obj
        Vector3 pos = transform.position;

        if (Vector3.Distance(pos, nextTile) <= moveSpeed)
        {
            return true;
        }

        // Partial step
        transform.position = Vector3.Lerp(pos, nextTile, moveSpeed * Time.deltaTime);

        Vector3 dir = GetDirectionTo(nextTile);
        
        return false;
    }
    private void JumpAlongPathWorking(Vector3 nextTile, float countStep, float maxStep)
    {
        // Readability obj
        Vector3 pos = transform.position;

        if (countStep == maxStep)
        {
            transform.position = nextTile;
        }

        // Partial step
        transform.position = Vector3.Lerp(pos, nextTile, (countStep / maxStep) * (Time.fixedDeltaTime / moveSpeed));

        Vector3 dir = GetDirectionTo(nextTile);
        // Update Animation too!
        animator.Animate(dir, 5000);
    }

    private Vector3 GetDirectionTo(Vector3 nextTile)
    {
        Vector3 dir = Vector3.zero;

        dir = nextTile - transform.position;

        return dir;
    }

    private IEnumerator MoveToNextTile(Vector3 NextTile)
    {
        path.RemoveAt(0);
        
        float step = .5f * (transform.position - NextTile).magnitude * Time.deltaTime;
        float t = 0f;
        while (t <= 1f)
        {
            t += .5f;
            transform.position = Vector3.Lerp(transform.position, NextTile, t);
            UpdateCreatureTile();
            Debug.Log("Moved to " + this.creatureTile.ToString());
            yield return new WaitForFixedUpdate();
        }
        transform.position = NextTile;
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
