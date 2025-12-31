using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializeSceneChanger : MonoBehaviour
{
    void Awake()
    {
        // シーンを切り替えてもこのオブジェクトが消えないようにする
        DontDestroyOnLoad(gameObject);
    }

    public void OnChangeCameraBySceneName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
