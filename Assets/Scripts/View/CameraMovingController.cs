using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovingController : MonoBehaviour
{
    const float Y_MIN = 0.0f;

    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        float y = Mathf.Max(Y_MIN, target.position.y);
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
