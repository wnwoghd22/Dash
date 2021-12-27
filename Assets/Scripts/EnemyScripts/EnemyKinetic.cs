using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKinetic : Enemy
{

    // Start is called before the first frame update
    protected override void Start()
    {
        player = FindObjectOfType<PlayerContoroller>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        MoveForward();
    }

    private void MoveForward()
    {
        if(!isFreezed)
        {
            Vector2 pos = transform.position;
            pos.x -= player.MoveSpeed * 10f;
            transform.position = pos;
        }
    }
}
