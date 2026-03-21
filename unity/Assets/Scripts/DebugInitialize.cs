using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugInitialize
{
    // AfterSceneLoad にすることで、最初のシーンが読み込まれた直後に判定します
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnAfterSceneLoad()
    {
#if UNITY_EDITOR
        string sceneName = SceneManager.GetActiveScene().name;

        // MainMenu 以外から開始された場合
        if (sceneName != "MainMenu")
        {
            SceneManager.LoadScene("MainMenu");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
#endif
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 読み込まれたのが MainMenu だったら
        if (scene.name == "MainMenu")
        {
            // イベント登録を解除（1回だけ実行するため）
            SceneManager.sceneLoaded -= OnSceneLoaded;

            // MainMenuNetworkRunnerManager を探して実行
            var manager = Object.FindFirstObjectByType<MainMenuNetworkRunnerManager>();
            if (manager != null)
            {
                manager.JoinMainGame();
            }
        }
    }
}