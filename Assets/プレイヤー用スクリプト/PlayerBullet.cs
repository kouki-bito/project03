using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float bulletspeed = 15f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Launch(bool isRight)
    {
        // �����ɍ��킹�đ��x��ݒ�i�E�Ȃ琳�A���Ȃ畉�j
        float direction = isRight ? 1f : -1f;
        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(direction * bulletspeed, 0);

        // �e�̌����ڂ����]������ꍇ
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }
}
