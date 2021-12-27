using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Ground : MonoBehaviour
{
    protected PlayerContoroller player;
    protected float leftBound;
    [SerializeField]
    protected float shiftSpeed = 5;

    protected Ground()
    {

    }

    protected virtual void Start()
    {
        Init();
    }
    protected virtual void Update() { }

    protected virtual void FixedUpdate()
    {
        Shift();
    }

    public virtual void Init()
    {
        player = FindObjectOfType<PlayerContoroller>();
        leftBound = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
    }

    private void Shift()
    {
        Vector2 pos = transform.position;
        pos.x -= player.MoveSpeed * shiftSpeed;
        transform.position = pos;

        if (transform.position.x < leftBound)
            Destroy(obj: gameObject);
    }
}
