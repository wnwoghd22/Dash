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

    public virtual void Init()
    {
        player = FindObjectOfType<PlayerContoroller>();
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) Debug.Log("failed");
        Debug.Log(rb);
        isReflected = false;
        vel = Vector2.zero;
    }

    public virtual void Reflect(Vector2 dir)
    {
        isReflected = true;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
        GetComponent<Rigidbody2D>().velocity = dir * reflectSpeed;
    }

    /// <summary>
    /// call freeze when player get into FOCUS state
    /// </summary>
    public void Freeze()
    {
        isFreezed = true;
        vel = GetComponent<Rigidbody2D>().velocity;
        GetComponent<Rigidbody2D>().Sleep();

        if (rb == null) Debug.Log("failed");
        Debug.Log(rb);
        //vel = rb.velocity;
        //rb.Sleep();
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
