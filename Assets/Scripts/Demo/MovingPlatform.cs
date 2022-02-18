using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private float topBound = 3f;
    [SerializeField]
    private float bottomBound = 0f;
    [SerializeField]
    private float moveSpeed = 0.1f;
    bool ascending = true;
    Transform tf;

    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        ascending = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = tf.position;
        if (ascending)
        {
            if (pos.y < topBound)
                pos.y += moveSpeed * Time.deltaTime;
            else
                ascending = false;
        } 
        else if (!ascending)
        {
            if (pos.y > bottomBound)
                pos.y -= moveSpeed * Time.deltaTime;
            else
                ascending = true;
        }
        tf.position = pos;
    }
}
