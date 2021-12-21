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
    private bool doubleJumped;

    private float fallMultiflier = 2.5f;
    private float lowJumpMultiflier = 2f;

    [SerializeField]
    private Transform[] groundPoints;
    private float overlapRadius = 0.1f;

    private float moveSpeed;
    public float MoveSpeed => moveSpeed;
    public const float RUNSPEED = 0.01f;

    private eState state;

    private Vector2 initAttackPos;
    private Vector2 endAttackPos;
    private bool isAttackPressed;
    private bool isAttack;
    private const float attackDelay = 0.5f;
    private const float slideDelay = 0.2f;
    private float attackDelta;
    [SerializeField]
    [Range(1, 10)]
    private float attackValocity;

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
        doubleJumped = false;
        isAttackPressed = false;
        isAttack = false;
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

    private void HandleAttackStickForPC()
    {
        if (Input.GetMouseButtonDown(0)) //attack phase starts
        {
            if (isAttack) return;
            initAttackPos = Input.mousePosition;
            isAttackPressed = true;
        }
        if (Input.GetMouseButtonUp(0)) //attack phase ends
        {
            if (!isAttackPressed) return;
            endAttackPos = Input.mousePosition;
            isAttackPressed = false;
            Vector2 attackDir = endAttackPos - initAttackPos;
            Attack(attackDir);
        }
    }
    private void HandleAttackStickForAndroid()
    {

    }
    private void Attack(Vector2 v)
    {
        v = v.normalized;

        // check if slide
        if (v.x < 0)
        {
            if (isOnGround())
            {
                state = eState.SLIDE;
                isAttack = true;
                attackDelta = slideDelay;
                moveSpeed *= 0.7f;
            }
            return;
        }

        // attack
        // add cos value to player speed
        state = eState.ATTACK;
        isAttack = true;
        attackDelta = attackDelay;
        moveSpeed = RUNSPEED * v.x * attackValocity;

        // add sin value to player object
        rb.velocity += Vector2.up * v.y * attackValocity;

    }

    private void HandleInput()
    {
        Debug.Log(state);
        switch (state)
        {
            case eState.RUN:
                HandleRunState();
                break;
            case eState.JUMP:
                HandleJumpState();
                break;
            case eState.ATTACK:
                HandleAttackState();
                break;
            case eState.SLIDE:
                HandleSlideState();
                break;
            case eState.FOCUS:
                break;
            default:
                break;
        }
    }

    private void HandleRunState()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isOnGround())
            {
                rb.velocity = Vector2.up * jumpValocity;
                state = eState.JUMP;
            }
            else if (!doubleJumped)
            {
                rb.velocity = Vector2.up * jumpValocity;
                state = eState.JUMP;
                doubleJumped = true;
            }
        }
        HandleAttackStickForPC();
    }
    private void HandleJumpState()
    {
        if (isOnGround())
        {
            state = eState.RUN;
            doubleJumped = false;
        }
        if (Input.GetButtonDown("Jump"))
        {
            if (!doubleJumped)
            {
                rb.velocity = Vector2.up * jumpValocity;
                doubleJumped = true;
            }
        }
        HandleAttackStickForPC();
    }
    private void HandleAttackState()
    {
        attackDelta -= 0.01f;
        if (attackDelta < 0)
        {
            moveSpeed = RUNSPEED;
            isAttack = false;
            if (isOnGround()) state = eState.RUN;
            else state = eState.JUMP;
        }
    }
    private void HandleSlideState()
    {
        attackDelta -= 0.01f;
        if (attackDelta < 0)
        {
            moveSpeed = RUNSPEED;
            isAttack = false;
            state = eState.RUN;
        }
    }
}
