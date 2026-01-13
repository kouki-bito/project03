using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
  public void LoadNextScene(string SceneName) 
    { 
    SceneManager.LoadScene(SceneName);
    }
}
