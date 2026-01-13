using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;

    void Update()
    {
        // ‰E•ûŒü‚ÖˆÚ“®
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    // ‰æ–ÊŠO‚Éo‚½‚çíœ
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
