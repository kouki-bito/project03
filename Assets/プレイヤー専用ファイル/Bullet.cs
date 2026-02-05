using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50f;
    private int direction = 1; // 1:‰E, -1:¶

    // ”­Ë‚ÉŒÄ‚Î‚ê‚é
    public void SetDirection(int dir)
    {
        direction = dir;
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * direction * Time.deltaTime);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    
}
