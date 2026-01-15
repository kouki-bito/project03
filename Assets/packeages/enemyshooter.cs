using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("【1】必須パーツ")]
    [SerializeField] private GameObject bulletPrefab;    // 弾のプレハブ
    [SerializeField] private Transform firePoint;        // 発射口（FirePoint）
    [SerializeField] private SpriteRenderer canonSprite; // 大砲の画像（Visual）

    [Header("【2】画像素材")]
    [SerializeField] private Sprite spriteHorizontal; // 真横(0度)のときの絵
    [SerializeField] private Sprite spriteAngled;     // 斜め(45度)のときの絵

    [Header("【3】設定")]
    [SerializeField] private float interval = 2.0f;        // 連射間隔
    [SerializeField] private float heightThreshold = 0.5f; // プレイヤーがどれくらい高いと反応するか

    private Transform player;
    private float timer;

    void Start()
    {
        // プレイヤーを自動で見つける
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        // --- 1. 高さ判定 ---
        // プレイヤーが、発射口より高い位置にいるか？
        bool isAbove = player.position.y > transform.position.y + heightThreshold;

        // --- 2. 砲身の角度と画像の切り替え ---
        if (isAbove)
        {
            // 【斜めモード】
            if (canonSprite != null) canonSprite.sprite = spriteAngled; // 絵を斜めに
            
            // 銃口を「親に対して」45度上げる
            if (firePoint != null)
            {
                firePoint.localRotation = Quaternion.Euler(0, 0, 45f);
            }
        }
        else
        {
            // 【真横モード】
            if (canonSprite != null) canonSprite.sprite = spriteHorizontal; // 絵を横に

            // 銃口を「親に対して」0度に戻す
            if (firePoint != null)
            {
                firePoint.localRotation = Quaternion.Euler(0, 0, 0f);
            }
        }

        // --- 3. 発射タイマー ---
        timer += Time.deltaTime;
        if (timer > interval)
        {
            Shoot();
            timer = 0;
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // FirePointが向いている角度(rotation)をそのまま弾に渡す
            // これで斜めの時は斜めに飛びます
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }

    // デバッグ用：黄色い線が出るので、FirePointの向きがわかります
    void OnDrawGizmos()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(firePoint.position, firePoint.right * 3.0f);
        }
    }
}