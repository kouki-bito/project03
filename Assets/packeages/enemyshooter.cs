using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("発射設定")]
    [SerializeField] private GameObject bulletPrefab; // 弾のプレハブ
    [SerializeField] private Transform firePoint;     // 弾が出る場所（口元）
    [SerializeField] private float interval = 2.0f;   // 何秒ごとに撃つか

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > interval)
        {
            Shoot();
            timer = 0;
        }
    }

    void Shoot()
    {
        if (bulletPrefab && firePoint)
        {
            // 弾を生成する
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}