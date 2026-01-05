using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpPower = 7f;
    public float climbSpeed = 4f;
    public float HP = 30;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isClimbing;
    private float defaultGravity;
    private bool isOnLadder;
    private Collider2D collider2D;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
    }

    void Update()
    {
        Move();
        Jump();
        Climb();


        

    }

    void Move()
    {
        float x = 0f;

        if (Input.GetKey(KeyCode.A))
            x = -1f;
        if (Input.GetKey(KeyCode.D))
            x = 1f;

        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isClimbing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            isGrounded = false; // ← 1回ジャンプ制御
        }
    }

    void Climb()
    {
        // 梯子に触れていて、上下キーを押したら登り状態にする
        if (isOnLadder && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)))
        {
            isClimbing = true;
            rb.gravityScale = 0f;
        }

        if (isClimbing)
        {
            float y = 0f;

            if (Input.GetKey(KeyCode.W))
                y = 1f;
            if (Input.GetKey(KeyCode.S))
                y = -1f;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, y * climbSpeed);
        }
    }

    // 地面判定
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isClimbing = true;
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            isOnLadder = true;
        }

        if (collision.CompareTag("Toge")) 
        {
            HP -= 5;
        }
    }

   
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isClimbing = false;
            rb.gravityScale = defaultGravity;
            isOnLadder = false;
            isClimbing = false;
            rb.gravityScale = defaultGravity;
        }
    }
}