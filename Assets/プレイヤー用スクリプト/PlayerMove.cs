//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;




//状態の遷移はAnimatorの中の矢印。
public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpPower = 7f;
    public float climbSpeed = 4f;
    public float HP = 30;
    public float MaxHP;
    public float damageInterval = 2f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isClimbing;
    private float defaultGravity;
    private bool isOnLadder;
    public Image HPBar;
    private bool isTouchingToge = false;
    private bool isDamageCooldown = false;
    // private Collider2D collider2D;

    SpriteRenderer sprite;
    private Animator anim;

    //public GameObject bulletPrefab; // インスペクターで弾のプレハブを割り当て
    //public Transform firePoint;    // 弾の発射位置（銃口など）

    void Start()
    {
        MaxHP = HP;
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Jump();
        Climb();
        //Shoot();

    }

    void Move()
    {
        // 入力取得（A/D）
        float x = Input.GetAxisRaw("Horizontal");

        if (Input.GetKey(KeyCode.A))
            x = -1f;
        if (Input.GetKey(KeyCode.D))
            x = 1f;

        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        if(x != 0)
        {
            anim.SetBool("isRunning", true);

            // 向き変更
            transform.localScale = new Vector3(
                Mathf.Sign(x), 1, 1
            );
        }

        else
        {
            anim.SetBool("isRunning", false);
        }

    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isClimbing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            isGrounded = false; // ← 1回ジャンプ制御
            anim.SetBool("isJumping00", true);
        }
        else anim.SetBool("isJumping00", false);
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
            anim.SetBool("isClimbing", true);
        }
        else anim.SetBool("isClimbing",false);
    }




    //void Shoot()
    //{
    //    GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
    //    PlayerBullet bulletScript = bulletObj.GetComponent<PlayerBullet>();

    //    // プレイヤーのlocalScale.xが正なら右向き、負なら左向きと判定
    //    bool isRight = transform.localScale.x > 0;

    //    bulletScript.Launch(isRight);
    //}

    // 地面判定
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.tag == "Toge")
        {
            isTouchingToge = true;
            if (!isDamageCooldown)
            {
                StartCoroutine(DamageLoop());
                HPBar.fillAmount = HP / MaxHP;
                anim.SetBool("isDamage", true);
            }
           
        }
       // else if (!(collision.gameObject.tag == "Toge")) anim.SetBool("isDamage", false);
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

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Toge"))
        {
            isTouchingToge = false;
            anim.SetBool("IsDamage",false);
        }

        if (!(collision.gameObject.tag == "Toge")) anim.SetBool("isDamage", false);
    }

    IEnumerator DamageLoop()
    {
        isDamageCooldown = true;

        while (isTouchingToge)
        {
            TakeDamage();
            yield return new WaitForSeconds(damageInterval);
        }

        isDamageCooldown = false;
    }

    void TakeDamage() 
    {
        HP -= 5;
        anim.SetBool("isDamage",true);
    }
}