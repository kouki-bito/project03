using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    // ===== 基本パラメータ =====
    public float moveSpeed = 10f;
    public float jumpPower = 10f;
    public float climbSpeed = 4f;
    //public float attackpower = 5f;

    // ===== HP関係 =====
    public float HP = 30;
    public float MaxHP;
    public float damageInterval = 2f;
    public Image HPBar;

    // ===== 武器関係 =====
    public GameObject Bullet;
    public float BulletSpeed = 50f;
    public AudioClip FiringSound; //発射音
    public Transform firePoint;        // 発射位置
    private bool isShot = true;

    // ===== 内部変数 =====
    private Rigidbody2D rb;
    private Animator anim;
    private AudioSource audioSource;

    private bool isGrounded = false;
    private bool isOnLadder = false;   // 梯子に触れている
    private bool isClimbing = false;   // 実際に登っている

    private float defaultGravity;

    // ダメージ関係
    private bool isTouchingToge = false;
    private Coroutine damageCoroutine;

    // =========================

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        MaxHP = HP;
        defaultGravity = rb.gravityScale;
    }

    void Update()
    {
        Move();
        Jump();
        Climb();
        if (Input.GetKeyDown(KeyCode.Return) && isShot)
        {
            StartCoroutine(AttackInterval());
        }
        UpdateHP();

        

        if (HP <= 0 ||gameObject.transform.position.y <= -170)
        {
            SceneManager.LoadScene("GameOver");
        }


    }

    // ===== 移動 =====
    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        if (x != 0)
        {
            anim.SetBool("isRunning", true);
            transform.localScale = new Vector3(Mathf.Sign(x), 1, 1);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
    }

    // ===== ジャンプ =====
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isClimbing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            isGrounded = false;
            anim.SetBool("isJumping00", true);
        }

        if (isGrounded)
        {
            anim.SetBool("isJumping00", false);
        }
    }

    // ===== 梯子処理 =====
    void Climb()
    {
        if (isOnLadder)
        {
            float y = Input.GetAxisRaw("Vertical");

            if (y != 0)
            {
                isClimbing = true;
                rb.gravityScale = 0f;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, y * climbSpeed);
                anim.SetBool("isClimbing", true);
            }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                anim.SetBool("isClimbing", false);
            }
        }
    }

    // ===== 攻撃 =====
    void Shot()
    {
       
            isShot = true;
            // プレイヤーの向きを取得
            int dir = transform.localScale.x > 0 ? 1 : -1;

            // 弾を生成
            GameObject bullet = Instantiate(
                Bullet,
                firePoint.position,
                Quaternion.identity
            );

            // 弾に向きを渡す
            bullet.GetComponent<Bullet>().SetDirection(dir);

            // 効果音
            audioSource.PlayOneShot(FiringSound);
           
        
    }

    // ===== HP表示 =====
    void UpdateHP()
    {
        HP = Mathf.Clamp(HP, 0, MaxHP);
        HPBar.fillAmount = HP / MaxHP;
    }

    // ===== トゲダメージ =====
    void TogeDamage()
    {
        HP -= 5;
        anim.SetBool("isDamage", true);
    }

    IEnumerator AttackInterval() 
    {
        isShot = false;
            Shot();
                yield return new WaitForSeconds(5f);
                isShot = true;
            
        
    }

    IEnumerator DamageLoop()
    {
        TogeDamage();

        while (isTouchingToge)
        {
            yield return new WaitForSeconds(damageInterval);
            if (isTouchingToge)
            {
                TogeDamage();
            }
        }
    }

    // ===== 地面判定 =====
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !isClimbing)
        {
            isGrounded = true;
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("衝突");
            HP -= 3;
            anim.SetBool("isDamage", true);
        }

        if (collision.gameObject.CompareTag("MidBoss"))
        {
            HP -= 5;
            anim.SetBool("isDamage", true);
        }

        if (collision.gameObject.CompareTag("LastBoss"))

        {
            HP -= 10;
            anim.SetBool("isDamage", true);
        }

    }

    // ===== Trigger処理 =====
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = true;
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
        }

        if (collision.CompareTag("Toge"))
        {
            isTouchingToge = true;

            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(DamageLoop());
            }
        }

        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("槍が刺さった！");
            
            HP -= 3; // ダメージ量（好きな数字でOK）
            anim.SetBool("isDamage", true);
            
            // 刺さった槍を消す
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("Apple"))
        {
            HP += 10;
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("GoldenApple"))
        {
            HP = MaxHP;
            Destroy(collision.gameObject);
        }

        
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = false;
            isClimbing = false;
            rb.gravityScale = defaultGravity;
            anim.SetBool("isClimbing", false);
        }

        if (collision.CompareTag("Toge"))
        {
            isTouchingToge = false;
            anim.SetBool("isDamage", false);

            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }

        
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("MidBoss") || collision.gameObject.CompareTag("LastBoss"))
            anim.SetBool("isDamage", false);
    }
}
