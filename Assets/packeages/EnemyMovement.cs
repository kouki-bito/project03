using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [Header("移動パラメータ")]
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private float rayDistance = 1.0f;     // 床センサーの長さ（下向き）
    [SerializeField] private float wallRayDistance = 0.5f; // 壁センサーの長さ（横向き）
    
    [Header("センサー位置調整")]
    [SerializeField] private float rayOffsetX = 0.5f;      // 前後（プラスで前）
    [SerializeField] private float rayOffsetY = -0.5f;     // 上下（マイナスで足元へ！）★追加

    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private int direction = 1; // 1:右, -1:左

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // ---------------------------------------------------------
        // センサーの基準点を作る（足元・少し前）
        // ---------------------------------------------------------
        Vector2 startPos = (Vector2)transform.position;
        startPos.x += rayOffsetX * direction; // 前に出す
        startPos.y += rayOffsetY;             // ★ここで高さを下げる！

        // 1. 崖チェック（基準点から「下」へ）
        RaycastHit2D cliffHit = Physics2D.Raycast(startPos, Vector2.down, rayDistance, groundLayer);

        if (cliffHit.collider == null)
        {
            Flip();
            return;
        }

        // ---------------------------------------------------------
        // 2. 壁チェック（目の前センサー）
        // ---------------------------------------------------------
        RaycastHit2D wallHit = Physics2D.Raycast(startPos, Vector2.right * direction, wallRayDistance, groundLayer);

        if (wallHit.collider != null)
        {
            GameObject hitObject = wallHit.collider.gameObject;

            // 【重要】自分自身、または「Player」には反応しない！
            // プレイヤーなら、壁だと思わずにそのまま突っ込む（移動処理へ進む）
            if (hitObject == gameObject || hitObject.CompareTag("Player") || hitObject.CompareTag("PlayerBullet"))
            {
                // 何もしない（スルーする）
            }
            else
            {
                // それ以外のもの（本物の壁や土管）なら反転する
                Flip();
                return;
            }
        }

        // 3. 移動
        rb.linearVelocity = new Vector2(speed * direction, rb.linearVelocity.y);
    }

    void Flip()
    {
        direction *= -1;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // デバッグ表示（Gizmos）も足元から出るように修正
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float dir = (Application.isPlaying) ? direction : (transform.localScale.x > 0 ? 1 : -1);

        // 基準点の計算
        Vector2 gizmoPos = (Vector2)transform.position;
        gizmoPos.x += rayOffsetX * dir;
        gizmoPos.y += rayOffsetY; // ★ここも下げる

        // 崖センサー（赤）
        Gizmos.DrawRay(gizmoPos, Vector2.down * rayDistance);

        // 壁センサー（青）
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(gizmoPos, Vector2.right * dir * wallRayDistance);
    }
}