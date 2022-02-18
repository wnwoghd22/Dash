using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoJump : MonoBehaviour
{
    private enum OnGroundType
    {
        CAST,
        THRESHOLD,
        ISTOUCHING,
    }

    Rigidbody2D rb;
    [SerializeField]
    private float moveSpeed = 3;

    [SerializeField]
    private OnGroundType groundType = OnGroundType.CAST;

    [SerializeField]
    private float threshold = 0.001f;
    [SerializeField]
    private ContactFilter2D groundFilter;

    [SerializeField]
    private JoyButton jumpButton;
    [SerializeField]
    private JoyStick moveStick;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleVirtualInput();
        
    }

    private void HandleKeyboardInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        Vector3 pos = transform.position;
        pos.x += h * moveSpeed * Time.deltaTime;
        transform.position = pos;

        if (Input.GetKeyDown(KeyCode.Space) && IsOnGround())
        {
            rb.velocity += Vector2.up * 10f;
        }
    }
    private void HandleVirtualInput()
    {
        float h = moveStick.InputDir.x;
        Vector3 pos = transform.position;
        pos.x += h * moveSpeed * Time.deltaTime;
        transform.position = pos;

        Debug.Log(jumpButton.State);

        if (jumpButton.State == eButtonState.Down && IsOnGround())
        {
            rb.velocity += Vector2.up * 10f;
        }
    }

    private bool IsOnGroundUsingCast => Physics2D.Raycast(transform.position, Vector3.down, 2f, 1 << LayerMask.NameToLayer("Ground")); 
    private bool IsOnGroundUsingThreshold => rb.velocity.y > -threshold && rb.velocity.y < threshold;
    private bool IsOnGroundUsingIsTouching => rb.IsTouching(groundFilter);

    private bool IsOnGround()
    {
        bool result = false;

        switch (groundType)
        {
            case OnGroundType.CAST:
                result = IsOnGroundUsingCast;
                Debug.DrawRay(transform.position, Vector3.down, Color.green, 0.1f);
                Debug.Log("is on ground using cast : " + result);
                break;
            case OnGroundType.THRESHOLD:
                result = IsOnGroundUsingThreshold;
                Debug.Log("is on ground using threshold(" + rb.velocity.y + ", " + threshold + ") : " + result);
                break;
            case OnGroundType.ISTOUCHING:
                result = IsOnGroundUsingIsTouching;
                Debug.Log("is on ground using is touching : " + result);
                break;
        }

        return result;
    }
}
