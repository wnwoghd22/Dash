using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Enemy
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        GetComponent<Rigidbody2D>().gravityScale = 0.0f;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
