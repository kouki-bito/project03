using UnityEngine;

public class Spear : MonoBehaviour
{
    [SerializeField] private float speed = 15f; // 飛ぶ速さ
    [SerializeField] private float lifeTime = 3f; // 消えるまでの時間

    void Start()
    {
        // 3秒経ったら自動で消える（ゴミ掃除）
        Destroy(gameObject, lifeTime);
    }

void Update()
    {
        // Translateは「自分の向いている方向」に進むので、
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        // ボス自身やセンサーには反応しないようにタグなどで制限すると良い
        if (other.CompareTag("Player") || other.CompareTag("Ground"))
        {
            Destroy(gameObject); // 自分を消す
        }
    }
}