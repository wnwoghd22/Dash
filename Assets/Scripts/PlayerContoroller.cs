using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public enum eState
{
    RUN,
    TAKEOFF, // first frame of jump
    JUMP,
    FALL,
    LAND, // about to ground
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
    private BoxCollider2D col;
    [SerializeField]
    private LayerMask jumpable;

    [SerializeField] private JoyButton jumpButton;
    [SerializeField] private JoyStick attackStick;
    private bool isAttack;
    private const float attackDelay = 1.0f;
    private Vector2 attackDir;
    [SerializeField] private JoyStick focusStick;
    private Vector2 reflectDir;
    private eState previousState;
    private float previousSpeed;
    private Enemy caughtEnemy;
    private PostProcessVolume volume;
    private EdgeDetect _edge;

    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector2 defaultPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.2f, Screen.height * 0.5f, 0));
        transform.position = defaultPos;
        moveSpeed = RUNSPEED;
        State = eState.RUN;
        doubleJumped = false;
        isAttack = false;
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

        HandleAttackStick();
        HandleFocusStick();
        HandleJumpButton();

        Debug.Log(State + " " + isOnGround);
    }

    public bool isOnGround => Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, .1f, jumpable);

    private void HandleState()
    {
        switch (State)
        {
            case eState.RUN:
                break;
            case eState.TAKEOFF:
                //jump effect
                State = eState.JUMP;
                break;
            case eState.JUMP:
                if (rb.velocity.y < 0)
                    State = eState.FALL;
                break;
            case eState.FALL:
                if (isOnGround)
                    State = eState.LAND;
                
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiflier - 1) * Time.deltaTime;
                break;
            case eState.LAND:
                //land effect
                animator.SetTrigger("run");
                doubleJumped = false;
                State = eState.RUN;
                break;
            case eState.ATTACK:
                attackDelta -= 0.1f;
                if (attackDelta < 0)
                {
                    isAttack = false;
                    attackCollider.SetActive(false);
                    if (isOnGround)
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
                if (rb.velocity.y > 0)
                    rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiflier - 1) * Time.deltaTime;
                break;
            case eButtonState.Down:
                if (isOnGround)
                {
                    State = eState.TAKEOFF;
                    rb.velocity = Vector2.up * jumpValocity;
                    animator.SetTrigger("jump");
                }
                else if (!doubleJumped && State == eState.JUMP)
                {
                    doubleJumped = true;
                    rb.velocity = Vector2.up * jumpValocity;
                    //animator.SetTrigger("jump");
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
    }
    
    private void HandleAttackStick()
    {
        switch (attackStick.State)
        {
            case eButtonState.None:
                break;
            case eButtonState.Down:
                attackDir = attackStick.InputDir;
                break;
            case eButtonState.Pressed:
                attackDir = attackStick.InputDir;

                // check if slide
                if (attackDir.x < 0)
                {
                    if (isOnGround && State != eState.SLIDE)
                    {
                        State = eState.SLIDE;
                        animator.SetBool("slide", true);
                    }
                }
                else
                {
                    if (State == eState.SLIDE)
                    {
                        animator.SetBool("slide", false);


                        if (isOnGround)
                            State = eState.RUN;
                        else
                            State = eState.JUMP;
                    }
                }
                break;
            case eButtonState.Up:
                animator.SetBool("slide", false);

                if (attackDir.x > 0)
                    Attack(attackDir);
                attackDir = Vector2.zero;
                break;
        }
    }
    private void Attack(Vector2 v)
    {
        v = v.normalized;

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
    /// <summary>
    /// recall at the end of animation "attack"
    /// </summary>
    public void ExitAttack()
    {

    }

    private void HandleFocusStick()
    {
        switch (focusStick.State)
        {
            case eButtonState.None:
                if (_edge.intensity.value > 0.0f)
                    _edge.intensity.value -= 0.03f;
                break;
            case eButtonState.Down:
                GetFocused();
                break;
            case eButtonState.Pressed:
                reflectDir = focusStick.InputDir;
                if (reflectDir.magnitude < 0.01f)
                    reflectDir = -Vector2.one;

                if (_edge.intensity.value < 0.7f)
                    _edge.intensity.value += 0.01f;
                break;
            case eButtonState.Up:
                ExitFocus();
                break;
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

        // catch enemy
        CatchEnemy();
    }
    private void ExitFocus()
    {
        animator.SetBool("focus", false);

        rb.WakeUp();
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies) enemy.Unfreeze();

        _edge.intensity.value = 0.0f;

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
        bool result = isOnGround;

        animator.SetBool("isOnGround", result);
        return result;
    }
    private void ExitReflect()
    {
        if (State == eState.FOCUS)
            return;

        Debug.Log("exit call");

        State = isOnGround ? eState.RUN : eState.JUMP;
    }
}
