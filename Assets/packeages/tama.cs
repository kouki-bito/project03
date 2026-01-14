using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float lifeTime = 3.0f; // 3秒で自動消滅

    void Start()
    {
        // 撃たれた方向に飛んでいくように、右方向へ力を加える
        // （発射時に回転させて向きを決めるため、常に「自分の右」に進めばOK）
        GetComponent<Rigidbody2D>().linearVelocity = transform.right * speed;
        
        // ずっと残り続けないように、時間で消す
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤーに当たったらダメージ（Player側の処理にお任せ）
        if (collision.CompareTag("Player"))
        {
            // ここに「プレイヤーにダメージを与える」処理を書く
            // 例: collision.GetComponent<PlayerHealth>().TakeDamage(1);
            Destroy(gameObject);
        }
        // 壁（Ground）に当たったら消える
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}