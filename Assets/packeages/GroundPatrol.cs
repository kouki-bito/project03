using UnityEngine;

public class GroundPatrol : MonoBehaviour
{
    [Header("動きの設定")]
    [SerializeField] private float speed = 2.0f;
    [SerializeField] private float range = 3.0f; // 歩く範囲

    private Vector3 startPos;
    private int direction = 1; // 1なら右、-1なら左
    private SpriteRenderer sr;

    void Start()
    {
        startPos = transform.position;
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 1. 移動する
        transform.Translate(Vector2.right * speed * direction * Time.deltaTime);

        // 2. 一定距離歩いたら折り返す
        float dist = transform.position.x - startPos.x;
        if (Mathf.Abs(dist) > range)
        {
            Flip();
        }
    }

    void Flip()
    {
        direction *= -1; // 進行方向を逆にする
        
        // 画像の向きも反転
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // ★重要：ボスが呼び出す「パワーアップ命令」
    public void PowerUp()
    {
        // スピード倍増！
        speed *= 2.0f;
        
        // 色を赤くして強そうにする
        if (sr != null)
        {
            sr.color = Color.red;
        }
    }
}