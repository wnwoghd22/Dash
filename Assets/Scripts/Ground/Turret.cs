using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Ground
{
    [SerializeField]
    private GameObject projectile;

    private const float fireTerm = 3.0f;
    private float fireDelta;

    private void Fire()
    {
        Vector2 playerDir = player.transform.position - transform.position;

        GameObject proj = Instantiate(projectile, transform.position, Quaternion.identity);
        proj.GetComponent<Projectile>().Init();

        //rotate
        float angle = Mathf.Atan2(playerDir.y, playerDir.x) * Mathf.Rad2Deg;
        proj.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);

        //add velocity
        proj.GetComponent<Rigidbody2D>().velocity += playerDir.normalized * 10;

    }

    protected override void Start()
    {
        base.Start();

        fireDelta = fireTerm;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (player.State != eState.FOCUS)
        {
            fireDelta -= 0.1f;

            if (fireDelta < 0f)
            {
                Fire();

                fireDelta = fireTerm;
            }
        }
            
    }
}
