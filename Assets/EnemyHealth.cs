using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField] private int maxHP = 1;
    private int currentHP;

    void Start()
    {
        currentHP = maxHP;
    }

    // Trigger（すり抜け）モード
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 念のためログ出し
        Debug.Log("接触(Trigger): " + collision.tag);

        // ★修正：Containsを使うことで、後ろにスペースが入っていても反応させる
        if (collision.tag.Contains("PlayerBullet"))
        {
            Hit(); // 共通のヒット処理へ
            Destroy(collision.gameObject); // 弾を消す
        }
    }

    // Collision（衝突）モード
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 念のためログ出し
        Debug.Log("接触(Collision): " + collision.gameObject.tag);

        // ★修正：Containsを使う
        if (collision.gameObject.tag.Contains("PlayerBullet"))
        {
            Hit(); // 共通のヒット処理へ
            Destroy(collision.gameObject);
        }
    }

    // ヒット時の処理をまとめました
    void Hit()
    {
        Debug.Log("命中！ダメージ処理を実行");
        TakeDamage(1);
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("敵は倒れた！");
        Destroy(this.gameObject);
    }
}