using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

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
    [SerializeField]
    private GameObject attackCollider;

    private Rigidbody2D rb;

    [SerializeField] private JoyButton jumpButton;
    private bool isJumpPressed;
    [SerializeField] private JoyStick attackStick;
    private bool isAttackPressed; // flag for detect the moment when up
    private bool isAttack;
    private const float attackDelay = 1.0f;
    private Vector2 attackDir;
    [SerializeField] private JoyStick focusStick;
    private bool isFocusPressed; // flag for detect the moment when up
    private Vector2 reflectDir;
    private eState previousState;
    private float previousSpeed;
    private float focusEffectDelta;
    private Enemy caughtEnemy;
    private PostProcessVolume volume;
    private EdgeDetect _edge;

    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector2 defaultPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.2f, Screen.height * 0.5f, 0));
        transform.position = defaultPos;
        moveSpeed = RUNSPEED;
        State = eState.RUN;
        doubleJumped = false;
        isJumpPressed = false;
        isAttackPressed = false;
        isAttack = false;
        isFocusPressed = false;
        caughtEnemy = null;
        attackCollider.SetActive(false);
        volume = FindObjectOfType<PostProcessVolume>();

        volume.profile.TryGetSettings(out _edge);

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleState();

        HandleAttackStickForAndroid();
        HandleFocusStick();
        HandleJumpButton();

        Debug.Log(jumpButton.State);
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
                    animator.SetTrigger("run");
                    doubleJumped = false;
                    State = eState.RUN;
                }
                break;
            case eState.ATTACK:
                attackDelta -= 0.1f;
                if (attackDelta < 0)
                {
                    attackCollider.SetActive(false);
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
        switch (jumpButton.State)
        {
            case eButtonState.None:
                break;
            case eButtonState.Down:
                if (isOnGround())
                {
                    State = eState.JUMP;
                    rb.velocity = Vector2.up * jumpValocity;
                    animator.SetTrigger("jump");
                }
                else if (!doubleJumped && State == eState.JUMP)
                {
                    doubleJumped = true;
                    rb.velocity = Vector2.up * jumpValocity;
                }
                break;
            case eButtonState.Pressed:
                break;
            case eButtonState.Up:
                break;
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiflier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && jumpButton.State != eButtonState.Pressed)
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
    private void HandleAttackStick()
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
                State = eState.SLIDE;
                animator.SetTrigger("slide");
                isAttack = true;
                attackDelta = slideDelay;
                moveSpeed *= 0.7f;
            }
            return;
        }

        // attack
        // add cos value to player speed
        State = eState.ATTACK;
        animator.SetTrigger("attack");
        isAttack = true;
        attackDelta = attackDelay;
        moveSpeed = RUNSPEED * v.x * attackValocity;

        // add sin value to player object
        rb.velocity += Vector2.up * v.y * attackValocity;

        attackCollider.SetActive(true);

        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        attackCollider.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

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

            if (_edge.intensity.value < 0.7f)
                _edge.intensity.value += 0.01f;
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
        animator.SetBool("focus", true);
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
        animator.SetBool("focus", false);

        rb.WakeUp();
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies) enemy.Unfreeze();

        _edge.intensity.value = 0.0f;

        Debug.Log(reflectDir);
        //focus action

        //if caught something -> reflect
        if (caughtEnemy != null)
        {
            animator.SetTrigger("reflect");
            reflectDir = reflectDir.normalized;
            isAttack = false; // give player extra chance to attack in the air

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

    private bool CheckIsOnGroundforAnimator()
    {
        bool result = isOnGround();

        animator.SetBool("isOnGround", result);
        return result;
    }
    private void ExitReflect()
    {
        if (isOnGround())
            State = eState.RUN;
        else
            State = eState.JUMP;
    }
}
