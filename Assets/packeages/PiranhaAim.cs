using UnityEngine;

public class PiranhaAim : MonoBehaviour
{
    [Header("画像の設定")]
    [SerializeField] private Sprite spriteSide; // 横向き (通常)
    [SerializeField] private Sprite spriteDiag; // 斜め上 (プレイヤーが上にいる時)

    [Header("発射口の設定")]
    [SerializeField] private Transform firePoint; // 弾が出る場所

    [Header("動作設定")]
    [SerializeField] private bool enableFlip = true;       // ★反転するかどうか
    [SerializeField] private float heightThreshold = 0.5f; // 高さ判定の感度

    private SpriteRenderer visualRenderer;
    private Transform playerTransform;

    void Start()
    {
        visualRenderer = GetComponentInChildren<SpriteRenderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 1. 左右の判定
        bool isLeft;

        if (enableFlip)
        {
            // ONの場合：プレイヤーの位置に合わせて自動で振り向く
            isLeft = playerTransform.position.x < transform.position.x;
            visualRenderer.flipX = isLeft;
        }
        else
        {
            // OFFの場合：Inspectorで設定された「Flip X」の状態をそのまま使う
            // (右向き固定ならfalse、左向き固定ならtrueのまま)
            isLeft = visualRenderer.flipX;
        }

        // 2. 上下の判定
        bool isAbove = playerTransform.position.y > (transform.position.y + heightThreshold);

        if (isAbove)
        {
            // --- 斜めモード ---
            visualRenderer.sprite = spriteDiag;
            SetMuzzleAngle(isLeft ? 135 : 45);
        }
        else
        {
            // --- 通常モード ---
            visualRenderer.sprite = spriteSide;
            SetMuzzleAngle(isLeft ? 180 : 0);
        }
    }

    void SetMuzzleAngle(float angleDeg)
    {
        if (firePoint != null)
        {
            firePoint.rotation = Quaternion.Euler(0, 0, angleDeg);
        }
    }
}