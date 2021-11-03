using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContoroller : MonoBehaviour
{
    [SerializeField]
    [Range(1, 10)]
    private float jumpValocity;

    private float fallMultiflier = 2.5f;
    private float lowJumpMultiflier = 2f;

    [SerializeField]
    private Transform[] groundPoints;
    private float overlapRadius = 0.1f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isOnGround())
                rb.velocity = Vector2.up * jumpValocity;
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiflier - 1) * Time.deltaTime;
        } 
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiflier - 1) * Time.deltaTime;
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.gameObject.layer == 13) //death
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        }
    }
}
