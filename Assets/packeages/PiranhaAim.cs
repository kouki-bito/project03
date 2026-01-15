using UnityEngine;

public class PiranhaAim : MonoBehaviour
{
    [Header("画像の設定")]
    [SerializeField] private Sprite spriteSide; // 横向き
    [SerializeField] private Sprite spriteDiag; // 斜め上

    [Header("発射口の設定")]
    [SerializeField] private Transform firePoint;

    [Header("動作設定")]
    [SerializeField] private bool enableFlip = true;      
    [SerializeField] private float heightThreshold = 0.5f;

    // ★重要設定：元の絵がどっちを向いているか？
    // 左向きの絵なら true、右向きの絵なら false にする
    [SerializeField] private bool originalSpriteIsLeft = true; 

    private SpriteRenderer visualRenderer;
    private Transform playerTransform;

    void Start()
    {
        visualRenderer = GetComponentInChildren<SpriteRenderer>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 1. 左右の判定（isLeft が true なら「左にいる」）
        bool isLeft;
        
        
        if (enableFlip)
        {
            isLeft = playerTransform.position.x < transform.position.x;
        }
        else
        {
            // flipXのチェックボックスがONなら「反転中」
            // 元の絵が左向きで、反転(flip)しているなら、今は「右」を向いている
            if (originalSpriteIsLeft)
                isLeft = !visualRenderer.flipX; 
            else
                isLeft = visualRenderer.flipX;
        }

        // 絵の反転処理
        // 「左を向きたい(isLeft)」かつ「元の絵が右向き」なら反転が必要、など
        if (originalSpriteIsLeft)
            visualRenderer.flipX = !isLeft; // 左絵なら、右を向くとき(false)に反転
        else
            visualRenderer.flipX = isLeft;  // 右絵なら、左を向くとき(true)に反転


        // 2. 上下の判定
        bool isAbove = playerTransform.position.y > (transform.position.y + heightThreshold);

        // 3. 角度と画像の設定
        if (isAbove)
        {
            // --- 斜めモード ---
            visualRenderer.sprite = spriteDiag;
            
            // ★修正ポイント：左なら135度、右なら45度
            SetMuzzleAngle(isLeft ? 135 : 45);
        }
        else
        {
            // --- 通常モード ---
            visualRenderer.sprite = spriteSide;

            // ★修正ポイント：左なら180度、右なら0度
            SetMuzzleAngle(isLeft ? 180 : 0);
        }
    }

    void SetMuzzleAngle(float angleDeg)
    {
        if (firePoint != null)
        {
            // Z軸を回転させる
            firePoint.rotation = Quaternion.Euler(0, 0, angleDeg);
        }
    }
}