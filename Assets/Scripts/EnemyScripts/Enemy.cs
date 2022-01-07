using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public abstract class Enemy : MonoBehaviour
{
    protected PlayerContoroller player;

    protected Rigidbody2D rb;

    public bool isReflected { get; protected set; }
    protected float reflectSpeed = 10f;

    protected bool isFreezed;
    protected Vector2 vel;

    protected Enemy()
    {

    }

    protected virtual void Start()
    {
        Init();
    }
    protected virtual void Update() { }

    protected virtual void FixedUpdate() { }

    public virtual void Init()
    {
        player = FindObjectOfType<PlayerContoroller>();
        rb = GetComponent<Rigidbody2D>();
        
        isReflected = false;
        vel = Vector2.zero;
    }

    public virtual void Reflect(Vector2 dir)
    {
        isReflected = true;
        Collider2D col = GetComponent<Collider2D>();
        if (!col.isTrigger) col.isTrigger = true;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
        GetComponent<Rigidbody2D>().velocity = dir * reflectSpeed;
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReflected)
        {
            if (collision.tag == "Enemy" || collision.tag == "Breakable")
            {
                Destroy(collision.gameObject);

                //explode effect

                Destroy(gameObject);
            }
            else if(collision.tag == "Ground")
            {
                //explode effect

                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// call freeze when player get into FOCUS state
    /// </summary>
    public void Freeze()
    {
        isFreezed = true;
        vel = GetComponent<Rigidbody2D>().velocity;
        GetComponent<Rigidbody2D>().Sleep();
    }
    public void Unfreeze()
    {
        isFreezed = false;
        GetComponent<Rigidbody2D>().WakeUp();
        GetComponent<Rigidbody2D>().velocity = vel;
        vel = Vector2.zero;
    }

    protected virtual void Fire() { }
}
