//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;




//状態の遷移はAnimatorの中の矢印。
public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float jumpPower = 10f;
    public float AttackPower = 10f;
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
    private bool isDamage = false;
    private bool isDamageCooldown = false;
    private bool isTouchingToge = false;
    private Coroutine damageCoroutine;
    // private Collider2D collider2D;

    SpriteRenderer sprite;
    private Animator anim;

    //武器関係
    public GameObject Bullet;
    public float BulletSpeed = 50;
    AudioSource audioSource;
    public AudioClip FiringSound;

    //public GameObject bulletPrefab; // インスペクターで弾のプレハブを割り当て
    //public Transform firePoint;    // 弾の発射位置（銃口など）

    void Start()
    {
        MaxHP = HP;
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        FiringSound = GetComponent<AudioClip>();
    }

    void Update()
    {
        Move();
        Jump();
        Climb();
        Shot();

        if (this.HP <= 0) 
        {
            SceneManager.LoadScene("GameOver");
        }

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
            //isClimbing = true;
            rb.gravityScale = 0f;
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

    void TogeDamage() 
    {
        HP -= 5;
        HP = Mathf.Max(HP, 0); // HPが0未満にならない
        HPBar.fillAmount = HP / MaxHP;
        anim.SetBool("isDamage", true);

    }

    IEnumerator DamageLoop() 
    {
        TogeDamage();
        while (isTouchingToge)
        {
            yield return new WaitForSeconds(damageInterval);

            if (isTouchingToge) TogeDamage();
            //else anim.SetBool("isDamage",false);
            
           }
    }

    void Shot() 
    {       if (Input.GetKeyDown(KeyCode.Return)) 
        {
            audioSource.PlayOneShot(FiringSound);
            
        }
    }


    void GoldenPower() 
    {
        HP = MaxHP;
        if (HP >= MaxHP) HP = MaxHP;
        jumpPower *= 2;
        moveSpeed *= 2;
        AttackPower *= 2;
    }



    // 地面判定
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
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

        if (collision.CompareTag("Toge"))
        {
            isTouchingToge = true;

            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(DamageLoop());
            }
        }

        if (collision.CompareTag("Apple")) 
        {
            if (HP < MaxHP) HP += 8;
            Destroy(collision.gameObject);
            if (HP >= MaxHP) HP = MaxHP;
        }

        if (collision.CompareTag("GoldenApple")) 
        { 
        
        }
    }



    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isClimbing = false;
            rb.gravityScale = defaultGravity;
            isOnLadder = false;
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
    
    }

   

    
}