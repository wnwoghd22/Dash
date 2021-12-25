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
    public const float RUNSPEED = 0.01f; // default run speed for reset speed becoming run state.

    private eState state;

    private Vector2 initAttackPos;
    private Vector2 endAttackPos;
    private const float slideDelay = 0.2f;
    private float attackDelta;
    [SerializeField]
    [Range(1, 10)]
    private float attackValocity;

    private Rigidbody2D rb;

    [SerializeField] private JoyButton jumpButton;
    private bool isJumpPressed;
    [SerializeField] private JoyStick attackStick;
    private bool isAttackPressed; // flag for detect the moment when up
    private bool isAttack;
    private const float attackDelay = 0.5f;
    private Vector2 attackDir;
    [SerializeField] private JoyStick focusStick;
    private bool isFocusPressed; // flag for detect the moment when up
    private Vector2 reflectDir;
    private eState previousState;
    private float previousSpeed;
    private float focusEffectDelta;

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
        isJumpPressed = false;
        isAttackPressed = false;
        isAttack = false;
        isFocusPressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleState();

        HandleAttackStickForAndroid();
        HandleFocusStick();
        HandleJumpButton();
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
    private void HandleState()
    {
        switch (state)
        {
            case eState.RUN:
                break;
            case eState.JUMP:
                if (isOnGround())
                {
                    Debug.Log("Land");
                    doubleJumped = false;
                    state = eState.RUN;
                }
                break;
            case eState.ATTACK:
                attackDelta -= 0.1f;
                if (attackDelta < 0)
                {
                    if (isOnGround())
                        state = eState.RUN;
                    else
                        state = eState.JUMP;
                    moveSpeed = RUNSPEED;
                    break;
                }
                break;
            case eState.SLIDE:
                break;
            case eState.FOCUS:
                break;
        }
    }

    private void HandleJumpButton()
    {
        if (Input.GetButtonDown("Jump") || jumpButton.Hold)
        {
            if(!isJumpPressed) //down
            {
                isJumpPressed = true;

                if (isOnGround())
                {
                    state = eState.JUMP;
                    rb.velocity = Vector2.up * jumpValocity;
                }
                else if (!doubleJumped && state == eState.JUMP)
                {
                    doubleJumped = true;
                    rb.velocity = Vector2.up * jumpValocity;
                }
            }
            
        }
        else if (!jumpButton.Hold)
        {
            if (isJumpPressed) //up
            {
                isJumpPressed = false;
            }
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiflier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && (!Input.GetButton("Jump") && !jumpButton.Hold))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiflier - 1) * Time.deltaTime;
        }
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
        if(attackStick.Hold)
        {
            if (!isAttackPressed) //down
                isAttackPressed = true;
            attackDir = attackStick.InputDir;
        }
        else
        {
            if (isAttackPressed) //up
            {
                isAttackPressed = false;
                Debug.Log(attackDir);

                Attack(attackDir);
                attackDir = Vector2.zero;
            }
        }
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

    private void HandleFocusStick()
    {
        if (focusStick.Hold)
        {
            if (!isFocusPressed) // stick down
            {
                previousState = state;
                state = eState.FOCUS;
                previousSpeed = moveSpeed;
                moveSpeed = 0.0f;
                // freezing

                isFocusPressed = true;
                // catch something
            }
            reflectDir = focusStick.InputDir;

            // set kernel effect parameter
            // float myValue = 1.0f;
            // Shader.SetGlobalFloat("_myValue", myValue);
        }
        else
        {
            if (isFocusPressed)
            {
                isFocusPressed = false;

                Debug.Log(reflectDir);
                //focus action
                //if caught something -> reflect
                //else
                moveSpeed = previousSpeed;
                state = previousState;

                reflectDir = Vector2.zero;
            }
        }
    }
}
