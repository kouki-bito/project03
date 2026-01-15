using UnityEngine;

public class tama : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;    // 弾の速さ
    [SerializeField] private float lifeTime = 3.0f; // 自然消滅までの時間

    void Start()
    {
        // 生まれた瞬間の「右方向（transform.right）」へ飛んでいく
        GetComponent<Rigidbody2D>().linearVelocity = transform.right * speed;
        
        // 時間が経ったら消す
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤーに当たったら
        if (collision.CompareTag("Player"))
        {
            // ★ダメージ処理は削除しました
            // プレイヤー側で感知してもらうために、自分（弾）だけ消す
            Destroy(gameObject); 
        }
        // 地面（壁）に当たったら
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject); // 壁にめり込まないように消す
        }
    }
}