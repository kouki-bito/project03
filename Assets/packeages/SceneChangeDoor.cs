using UnityEngine;
using UnityEngine.SceneManagement; // ★これが必要！

public class SceneChangeDoor : MonoBehaviour
{
    [Header("移動先のシーン名")]
    [SerializeField] private string nextSceneName = "Stage2"; // 初期値

    // 当たり判定（Trigger）に入った瞬間に呼ばれる
    void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤーが触れた時だけ反応する
        if (collision.CompareTag("Player"))
        {
            // 指定した名前のシーンを読み込む
            SceneManager.LoadScene(nextSceneName);
        }
    }
}