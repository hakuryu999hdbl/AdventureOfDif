using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed;
    public float jumpForce;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void Update()
    {
        CheckInput();
    }//输入用Update（听）

    public void FixedUpdate()
    {
        Movement();
        Jump();
    }//每帧执行动作用FixedUpdate（做）

    void Movement()
    {
        //float horizontalInput = Input.GetAxis("Horizontal"); // -1 ~ 1 包括小数
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // -1 ~ 1

        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

        if (horizontalInput != 0)
        {
            transform.localScale = new Vector3(horizontalInput, 1, 1);
        }
    }

    private bool jumpPressed;

    void CheckInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }
    }

    void Jump()
    {
        if (jumpPressed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpPressed = false; // 提取文字中未显示，但逻辑上建议加上，防止无限跳跃
        }
    }
}
