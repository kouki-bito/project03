using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public int HP = 30;
    public float speed = 10f;
    public float JumpPower = 10f;

    private Rigidbody2D rb;
    private Vector2 move;

    private bool isLadder = false;
    private float defaultGravity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
    }

    void Update()
    {
        float moveX = 0f;
        float moveY = 0f;

        // 横移動
        if (Input.GetKey(KeyCode.D)) moveX = 1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;

        // ジャンプ（梯子中は不可）
        if (!isLadder && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * JumpPower, ForceMode2D.Impulse);
        }

        // 梯子移動
        if (isLadder)
        {
            if (Input.GetKey(KeyCode.W)) moveY = 1f;
            if (Input.GetKey(KeyCode.S)) moveY = -1f;
        }

        move = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("ladder"))
        {
            isLadder = true;
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("ladder"))
        {
            isLadder = false;
            rb.gravityScale = defaultGravity;
        }
    }
}
