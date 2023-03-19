using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [SerializeField]
    private Vector3 gridPosition;
    [SerializeField]
    private SpriteRenderer sprite;
    [SerializeField]
    private float moveSpeed = 4f;
    [SerializeField]
    private bool selected = false;
    [SerializeField]
    private CreatureAnimator animator = null;

    [SerializeField]
    private bool DebuggingMovement = false;
    [SerializeField]
    private Vector3 DebugMoveTarget = new Vector3(5f, 3f);


    private Vector3 targetPosition;
    private float stopDistance = 0.1f;

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
        if (LevelGrid.Instance != null)
        {
            gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            LevelGrid.Instance.AddCreatureAtGridPosition(gridPosition, this);
        }
        else
        {
            gridPosition = new Vector3(.5f, .5f);
        }

        if (DebuggingMovement)
        {
            Move(DebugMoveTarget);
        }
    }

    private void Move(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, targetPosition) > stopDistance)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            float t = Time.deltaTime;
            transform.position += moveDirection * moveSpeed * t;
            this.animator.Animate(moveDirection, t * moveSpeed);
        }
        else
        {
            this.animator.StopAnimating();
        }

    }
}
