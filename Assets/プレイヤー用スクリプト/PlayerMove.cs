using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public int HP = 30;
    public float speed = 10f;
    public float jumpPower = 10f;

    private Rigidbody2D rb;
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
        float moveY = rb.linearVelocity.y;

        // ���ړ�
        if (Input.GetKey(KeyCode.D)) moveX = 1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;

        // �W�����v�i��q���͕s�j
        if (!isLadder && Input.GetKeyDown(KeyCode.Space))
        {
            moveY = jumpPower;
        }

        // ��q�ړ�
        if (isLadder)
        {
            moveY = 0f;
            if (Input.GetKey(KeyCode.W)) moveY = speed;
            if (Input.GetKey(KeyCode.S)) moveY = -speed;
        }

        rb.linearVelocity = new Vector2(moveX * speed, moveY);
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
