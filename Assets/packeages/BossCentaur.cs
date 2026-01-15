using UnityEngine;
using System.Collections;

public class BossCentaur : MonoBehaviour
{



[SerializeField] private string nextSceneName = "ClearScene"; // ★追加：移動先のシーン名

    private bool isDead = false; // ★追加：死んだかどうかのフラグ

    [Header("基本ステータス")]
    [SerializeField] private float dashSpeed = 10f; // 突進の速さ
    [SerializeField] private float jumpPower = 12f; // ジャンプ力

    [Header("攻撃の激しさ（待機時間）")]
    [SerializeField] private float normalInterval = 2.0f; // 通常時の休憩時間
    [SerializeField] private float angryInterval = 0.8f;  // ★ピンチ時（HP半分以下）の休憩時間

    [Header("攻撃1：やり投げ")]
    [SerializeField] private GameObject spearPrefab; // 槍のプレハブ
    [SerializeField] private Transform throwPoint;   // 槍が出る場所（手元）

    [Header("攻撃2：手下召喚")]
    [SerializeField] private GameObject minionPrefab; // 召喚する雑魚（GroundPatrol付き）
    [SerializeField] private Transform[] spawnPoints; // 出現場所リスト（天井など）
    [SerializeField] private int maxMinions = 3;

    private Rigidbody2D rb;
    private Animator anim;
    private Transform player;
    private EnemyHealth health; // HPを見るために必要
    private bool isActing = false; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        health = GetComponent<EnemyHealth>(); 
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        StartCoroutine(ThinkRoutine());
    }

    void Update()
    {
      if (health != null && health.currentHP <= 0 && !isDead)
        {
            isDead = true; // 1回だけ実行するためのロック
            
            // 動きを止める
            StopAllCoroutines();
            rb.linearVelocity = Vector2.zero;
            anim.speed = 0; // アニメも止める（必要なら）

            // 別ファイルにある SceneChange を探して実行！
            SceneChange sceneChanger = FindObjectOfType<SceneChange>();
            if (sceneChanger != null)
            {
                sceneChanger.LoadNextScene(nextSceneName);
            }
            else
            {
                Debug.LogError("シーンに SceneChange スクリプトがついたオブジェクトがありません！");
            }
            return; 
        
        }
        if (!isActing && player != null)
        {
            float dist = player.position.x - transform.position.x;
            if (Mathf.Abs(dist) > 0.5f) 
            {
                FaceDirection(dist > 0);
            }
        }
    }

    // --- ボスの思考回路（メインループ） ---
    IEnumerator ThinkRoutine()
    {
        while (true) 
        {
            // 1. 待機（休憩）
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("Move", false);

            float currentWaitTime = normalInterval;

     
            if (health != null && health.currentHP <= health.maxHP / 2)
            {
                currentWaitTime = angryInterval;
                
          
                StartCoroutine(FlashRedEffect());
            }

            yield return new WaitForSeconds(currentWaitTime);

            isActing = true;
            float distToPlayer = (player != null) ? Vector2.Distance(transform.position, player.position) : 10f;
            int dice = Random.Range(0, 100); // 0〜99のサイコロ

            // 【行動パターンの分岐】
            if (dice < 20)
            {
                // 20%：手下召喚（ただし画面に敵が多すぎたら槍投げに変更）
                if (GameObject.FindGameObjectsWithTag("Enemy").Length <= maxMinions)
                    yield return StartCoroutine(SummonMinion());
                else
                    yield return StartCoroutine(ThrowSpear());
            }
            else if (distToPlayer < 5.0f && dice < 60)
            {
                // 近距離 かつ 60%未満：ジャンププレス
                yield return StartCoroutine(JumpPress());
            }
            else if (dice < 80)
            {
                // 80%未満：突進
                yield return StartCoroutine(DashAttack());
            }
            else
            {
                yield return StartCoroutine(ThrowSpear());
            }

  
            isActing = false;
        }
    }

    IEnumerator ThrowSpear()
    {
        anim.SetTrigger("Attack"); // 投げるモーション
        yield return new WaitForSeconds(0.5f); // 予備動作

        if (spearPrefab && throwPoint)
        {
            bool isFacingLeft = transform.localScale.x < 0;


            Quaternion throwRotation = isFacingLeft ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

            Instantiate(spearPrefab, throwPoint.position, throwRotation);
        }

        yield return new WaitForSeconds(1.0f); // 投げた後の硬直
    }

    // 技B: 突進
    IEnumerator DashAttack()
    {
        // 足踏みなどの予兆
        yield return new WaitForSeconds(0.5f); 
        
        anim.SetBool("Move", true); // 走るアニメON
        
        // 向いている方向に猛ダッシュ
        float dir = transform.localScale.x > 0 ? 1 : -1;
        rb.linearVelocity = new Vector2(dir * dashSpeed, 0);

        yield return new WaitForSeconds(1.5f); // 1.5秒走り続ける
        
        rb.linearVelocity = Vector2.zero; // 急停止
        anim.SetBool("Move", false); // 走るアニメOFF
    }

    // 技C: ジャンププレス

    IEnumerator JumpPress()
    {
        anim.SetTrigger("Jump");
        
        float dir = (player.position.x > transform.position.x) ? 1 : -1;
        rb.linearVelocity = new Vector2(dir * 3.0f, jumpPower);

        yield return new WaitForSeconds(0.5f);
        
        // ★修正：タイムアウト設定（最大3秒待つ）
        float timeOut = 3.0f; 
        
        // 地面に着くまで、かつ タイムアウトになるまで待機
        while (!IsGrounded() && timeOut > 0)
        {
            timeOut -= Time.deltaTime; // 時間を減らす
            yield return null;
        }
        
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(1.0f);
    }

    // 技D: 強化手下の召喚
    IEnumerator SummonMinion()
    {
    
        anim.SetTrigger("Attack"); 

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        sr.color = Color.red;

        yield return new WaitForSeconds(1.0f); // 溜め時間

        if (minionPrefab && spawnPoints.Length > 0)
        {
            // ランダムな場所を選ぶ
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            
            // 手下を生成
            GameObject minion = Instantiate(minionPrefab, point.position, Quaternion.identity);
            
            // ★手下のスクリプトを取得して「パワーアップ」命令を出す
            GroundPatrol patrol = minion.GetComponent<GroundPatrol>();
            if (patrol != null)
            {
                patrol.PowerUp(); // 「強くなれ！」
            }
        }

        sr.color = Color.white; // 色を戻す
        yield return new WaitForSeconds(0.5f);
    }

    void FaceDirection(bool isRight)
    {
        Vector3 scale = transform.localScale;
        scale.x = isRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    // 地面に足がついているかチェック
    bool IsGrounded()
    {
        // 足元から少し下に線を引いて確認
        return Physics2D.Raycast(transform.position, Vector2.down, 1.5f, LayerMask.GetMask("Ground"));
    }

    // 怒り演出（一瞬赤くなる）
    IEnumerator FlashRedEffect()
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(1f, 0.5f, 0.5f); // 薄い赤
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.white;
        }
    }
}