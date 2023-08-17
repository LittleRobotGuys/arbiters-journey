using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.TerrainUtils;
using DebugUtils;

public class Creature : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer sprite;
    [SerializeField]
    private bool selected = false;
    [SerializeField]
    private SpriteAnimator animator = null;

    [SerializeField]
    private Pathfinding pathfinding;

    private SmartTile creatureTile;
    private List<PathNode> path;

    [SerializeField]
    private SmartTileMap groundTileMap;
    [SerializeField] 
    private SmartTileMap collisionTileMap;
    private Vector3 IMPOSSIBLE_V3 = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    
    // Interactions
    [SerializeField]
    private Interactable interactable;

    // Movement stuff
    [SerializeField]
    private float moveSpeed = .25f;
    bool readyForNextTile = true;

    // Audio
    public AudioSource audioSrc;
    public AudioClip GrassClip1;
    public AudioClip GrassClip2;
    public AudioClip StoneClip1;
    public AudioClip StoneClip2;
    public AudioClip GravelClip1;
    public AudioClip GravelClip2;
    public AudioClip SnowClip1;
    public AudioClip SnowClip2;

    private bool leftFoot = false;

    private bool isMuted = true;


    private void Awake()
    {
        if (sprite == null)
        {
            sprite = GetComponentInChildren<SpriteRenderer>();
        }

        if (animator == null)
        {
            animator = gameObject.GetComponent<SpriteAnimator>();
        }

        if (interactable == null)
        {
            interactable = gameObject.GetComponent<Interactable>();
            if (interactable == null)
            {
                interactable = gameObject.AddComponent<Interactable>();

            }
        }

        interactable.SetDefaultNPC();
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
        UpdateCreatureTile();
        isMuted = false;
    }

    public void UpdateCreatureTile()
    {
        var pos = transform.position;
        creatureTile = groundTileMap.GetTileSansOffset(groundTileMap.WorldToCell(pos));

        if (creatureTile != null)
        {
            creatureTile.ClearInventory();
            creatureTile.AddInventory(gameObject);
            PlayTileSound(creatureTile.name.ToLower());
        }
    }

    private void RequestCardinalMap()
    {

        if (creatureTile.x == groundTileMap.GetOffset().x * groundTileMap.GetXRadius() + groundTileMap.GetXRadius() - 1)
        {
            LeaveTile();
            groundTileMap.MoveCameraEast(this);
        }
        else if (creatureTile.x == groundTileMap.GetOffset().x * groundTileMap.GetXRadius() + 1)
        {
            LeaveTile();
            groundTileMap.MoveCameraWest(this);
        }
        // Need similar checking for up and down
        else if (creatureTile.y >= 8)
        {
            LeaveTile();
            groundTileMap.MoveCameraNorth(this);
        }
        else if (creatureTile.y <= -8)
        {
            LeaveTile();
            groundTileMap.MoveCameraSouth(this);
        }
    }

    private SmartTile LeaveTile()
    {
        if (creatureTile != null)
        {
            creatureTile.ClearInventory();
        }
        return creatureTile;
    }

    private void PlayTileSound(string v)
    {
        if (isMuted) return;

        AudioClip clip = null;
        if (v.Contains("grass"))
        {
            clip = leftFoot ? GrassClip1 : GrassClip2;
        }
        else if (v.Contains("stone"))
        {
            clip = leftFoot ? StoneClip1 : StoneClip2;
        }
        else if (v.Contains("gravel"))
        {
            clip = leftFoot ? GravelClip1 : GravelClip2;
        }
        else if (v.Contains("snow"))
        {
            clip = leftFoot ? SnowClip1 : SnowClip2;
        }
        audioSrc.PlayOneShot(clip);
        leftFoot = !leftFoot;
    }

    void Update()
    {
        if (transform.position.x > 100 || transform.position.x < -100)
        {
            Debug.Log("HERE");
        }
    }

    public void TileSelected(SmartTile tile)
    {
        // Default audio plays on click
        audioSrc.Play();

        if (tile != null)
        {
            if (creatureTile == null)
            {
                UpdateCreatureTile();
            }
            if (tile.ToString() != this.creatureTile.ToString())
            {
                path = pathfinding.FindPath(this.creatureTile, tile);
                if (path != null) StartCoroutine(MoveToTile());
                else Debug.LogWarning("Something happened when trying to get to " + tile.ToString() + " from " + creatureTile.ToString());
            }
            else
            {
                Debug.Log(name + " cannot move to tile, already there.");
                return;
            }
        }
    }
    
    private IEnumerator MoveToTile()
    {
        readyForNextTile = true;
        SmartTile lastTile = null;

        Vector3 NextTile = this.IMPOSSIBLE_V3;
        for (int t = 1; t < path.Count;)
        {
            NextTile = path[t].GetLocationAsV3();

            if (readyForNextTile)
            {
                lastTile = LeaveTile();
                TweenAlongPath(NextTile);
                UpdateCreatureTile();
                t++;
            }

            yield return new WaitForEndOfFrame();
        }

        // Finalization of placement ensures Character ends up where they should be even in edge cases.
        LeaveTile();
        transform.position = NextTile;
        UpdateCreatureTile();
        animator.ResetAnimation();
        RequestCardinalMap();
        path = null;
    }
    private void TweenAlongPath(Vector3 path)
    {
        // DOMove(end,duration, snapping)
        readyForNextTile = false;
        animator.Animate(path - transform.position, moveSpeed);
        transform.DOMove(path, moveSpeed, true).OnComplete(() => readyForNextTile = true);
        animator.StopAnimating();
    }


    public Pathfinding GetPathfinding()
    {
        return this.pathfinding;
    }

    internal SmartTile GetTile()
    {
        return this.creatureTile;
    }

    internal bool IsSelected()
    {
        return selected;
    }
}
