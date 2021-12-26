using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatic : Enemy
{

    [SerializeField]
    private GameObject projectile;

    private const float fireTerm = 3.0f;
    private float fireDelta;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        fireDelta = fireTerm;
        GetComponent<Rigidbody2D>().gravityScale = 0.0f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!isFreezed)
        {
            fireDelta -= 0.1f;
            if (fireDelta < 0)
            {
                fireDelta = fireTerm;
                Fire();
            }
        }

    }

    protected override void Fire()
    {
        Vector2 playerDir = player.transform.position - transform.position;

        GameObject proj = Instantiate(projectile, transform.position, Quaternion.identity);
        proj.GetComponent<Enemy>().Init();

        //rotate
        float angle = Mathf.Atan2(playerDir.y, playerDir.x) * Mathf.Rad2Deg;
        proj.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);

        //add velocity
        proj.GetComponent<Rigidbody2D>().velocity += playerDir;

    }
}
