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

    public eState State { get; private set; }

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
    private Enemy caughtEnemy;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = RUNSPEED;
        State = eState.RUN;
        doubleJumped = false;
        isJumpPressed = false;
        isAttackPressed = false;
        isAttack = false;
        isFocusPressed = false;
        caughtEnemy = null;
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
        switch (State)
        {
            case eState.RUN:
                break;
            case eState.JUMP:
                if (isOnGround())
                {
                    Debug.Log("Land");
                    doubleJumped = false;
                    State = eState.RUN;
                }
                break;
            case eState.ATTACK:
                attackDelta -= 0.1f;
                if (attackDelta < 0)
                {
                    if (isOnGround())
                        State = eState.RUN;
                    else
                        State = eState.JUMP;
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
                    State = eState.JUMP;
                    rb.velocity = Vector2.up * jumpValocity;
                }
                else if (!doubleJumped && State == eState.JUMP)
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
                State = eState.SLIDE;
                isAttack = true;
                attackDelta = slideDelay;
                moveSpeed *= 0.7f;
            }
            return;
        }

        // attack
        // add cos value to player speed
        State = eState.ATTACK;
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
                GetFocused();
            reflectDir = focusStick.InputDir;

            // set kernel effect parameter
            // float myValue = 1.0f;
            // Shader.SetGlobalFloat("_myValue", myValue);
        }
        else
        {
            if (isFocusPressed)
            {
                ExitFocus();
            }
        }
    }
    private void GetFocused()
    {
        previousState = State;
        State = eState.FOCUS;
        previousSpeed = moveSpeed;
        moveSpeed = 0.0f;
        // freezing
        rb.Sleep();
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies) enemy.Freeze();

        isFocusPressed = true;

        // catch enemy
        CatchEnemy();
    }
    private void ExitFocus()
    {
        isFocusPressed = false;
        rb.WakeUp();
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies) enemy.Unfreeze();

        Debug.Log(reflectDir);
        //focus action

        //if caught something -> reflect
        if (caughtEnemy != null)
        {
            reflectDir = reflectDir.normalized;

            caughtEnemy.Reflect(-reflectDir);
            //moveSpeed = RUNSPEED * reflectDir.x * attackValocity;
            moveSpeed = previousSpeed;
            rb.velocity += Vector2.up * reflectDir.y * attackValocity;
        }
        else
        {
            moveSpeed = previousSpeed;
        }
        State = previousState;

        reflectDir = Vector2.zero;
    }
    /// <summary>
    /// try to find the nearest enemy object and catch it
    /// </summary>
    /// <returns>returns true if player catch enemy</returns>
    private bool CatchEnemy()
    {
        //find closest
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 2f);
        if (enemies.Length == 0)
            return false;

        float minDistance = Mathf.Infinity;
        foreach(Collider2D collider in enemies)
        {
            GameObject enemy = collider.gameObject;
            if (enemy.GetComponent<Enemy>() == null)
                continue;
            float distance = Mathf.Abs((enemy.transform.position - transform.position).magnitude);
            if (distance < minDistance)
                caughtEnemy = enemy.GetComponent<Enemy>();
        }

        return caughtEnemy != null;
    }
}
