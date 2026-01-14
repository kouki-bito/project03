using UnityEngine;

public class EnemyFly : MonoBehaviour
{
    [Header("動きの設定")]
    [SerializeField] private float width = 3.0f;   // 横に動く幅 (0なら動かない)
    [SerializeField] private float height = 0.0f;  // 縦に動く幅 (0なら動かない)
    [SerializeField] private float speed = 2.0f;   // 全体の速さ

    [Header("向きの設定")]
    [SerializeField] private bool faceRight = false; // チェックを入れると、右を向いて始まる

    private Vector3 startPos;
    private float timeOffset;

    void Start()
    {
        startPos = transform.position;
        // 敵ごとにタイミングをずらす
        timeOffset = Random.Range(0f, 2f);
    }

    void Update()
    {
        // 1. 時間の経過（Sin/Cosを使って、滑らかな往復を作る）
        float cycle = (Time.time + timeOffset) * speed;

        // 2. 新しい位置を計算
        // Xは Cos(コサイン)、Yは Sin(サイン) を使うと、両方設定した時に「円」を描けます
        float newX = startPos.x + Mathf.Cos(cycle) * width;
        float newY = startPos.y + Mathf.Sin(cycle) * height;

        // 3. 移動実行
        transform.position = new Vector3(newX, newY, startPos.z);

        // 4. 自動で向きを変える（横移動がある時だけ）
        if (width > 0)
        {
            // Cosの結果から、今「右に行こうとしてる」か「左に行こうとしてる」か判定
            // マイナスの時は左、プラスの時は右
            float moveDir = -Mathf.Sin(cycle); 

            Flip(moveDir);
        }
    }

    // 向きを反転させる処理
    void Flip(float velocity)
    {
        // ほとんど動いていない時は無視
        if (Mathf.Abs(velocity) < 0.1f) return;

        Vector3 scale = transform.localScale;

        if (velocity > 0)
        {
            // 右に進んでいる（スケールを正にする）
            scale.x = Mathf.Abs(scale.x);
        }
        else
        {
            // 左に進んでいる（スケールを反転）
            scale.x = -Mathf.Abs(scale.x);
        }

        transform.localScale = scale;
    }
}