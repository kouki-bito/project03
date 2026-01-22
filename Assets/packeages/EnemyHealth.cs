using UnityEngine;

[SelectionBase]
public class EnemyHealth : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField] public int maxHP = 1;
    public int currentHP;

    [Header("設定")]
    // ★追加：これがONなら死んだらすぐ消える（雑魚用）
    // OFFなら消えない（ボス用。演出はボス側のスクリプトに任せる）
    [SerializeField] private bool destroyOnDeath = true; 

    void Start()
    {
        currentHP = maxHP;
    }

    // Trigger（すり抜け）モード
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("接触(Trigger): " + collision.tag);

        if (collision.tag.Contains("PlayerBullet"))
        {
            Hit(); 
            Destroy(collision.gameObject); // 弾を消す
        }
    }

    // Collision（衝突）モード
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Debug.Log("接触(Collision): " + collision.gameObject.tag);

        if (collision.gameObject.tag.Contains("PlayerBullet"))
        {
            Hit(); 
            Destroy(collision.gameObject);
        }
    }

    void Hit()
    {
        // Debug.Log("命中！ダメージ処理を実行");
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
        Debug.Log("HPが0になった！");

        // ★ここが重要！
        // 「死んだら破壊する設定」になっている時だけ消す。
        // ボスはこの設定をOFFにするので、ここでは消されずに生き残る！
        if (destroyOnDeath)
        {
            Destroy(this.gameObject, 0.01f);
        }
    }
}