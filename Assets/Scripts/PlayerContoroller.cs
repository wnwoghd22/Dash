using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eState
{
    RUN,
    JUMP,
    ATTACK,
    SLIDE,
    FOCUS, // if focus button pressed
}

public class PlayerContoroller : MonoBehaviour
{
    [SerializeField]
    [Range(1, 10)]
    private float jumpValocity;

    private float fallMultiflier = 2.5f;
    private float lowJumpMultiflier = 2f;

    [SerializeField]
    private Transform[] groundPoints;
    private float overlapRadius = 0.1f;

    private float moveSpeed;
    public float MoveSpeed => moveSpeed;
    private const float RUNSPEED = 0.01f;

    private eState state;

    private Vector2 initAttackPos;
    private Vector2 endAttackPos;

    private bool isAttack;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = RUNSPEED;
        state = eState.RUN;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();

        if (Input.GetButtonDown("Jump"))
        {
            if (isOnGround())
                rb.velocity = Vector2.up * jumpValocity;
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiflier - 1) * Time.deltaTime;
        } 
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiflier - 1) * Time.deltaTime;
        }

        if (isAttack)
        {
            // how can I handle attack direction?
        }
    }

    private bool isOnGround()
    {
        if (rb.velocity.y <= 0)
        {
            foreach (Transform point in groundPoints)
            {
                Collider2D[] collider2s = Physics2D.OverlapCircleAll(point.position, overlapRadius);
                for (int i = 0; i < collider2s.Length; ++i)
                {
                    if (collider2s[i].gameObject != gameObject)
                        return true;
                }
            }
        }

        return false;
    }

    private void CalculateAttackSpeed(Vector2 v)
    {

    }

    private void HandleInput()
    {
        switch (state)
        {
            case eState.RUN:
                HandleRunState();
                break;
            case eState.JUMP:
                break;
            case eState.ATTACK:
                break;
            case eState.SLIDE:
                break;
            case eState.FOCUS:
                break;
            default:
                break;
        }
    }

    private void HandleRunState()
    {

    }
}
