using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainControllerNetworkRunnerManager : MonoBehaviour
{
    [SerializeField]
    private NetworkRunner _networkRunnerPrefab;

    private async void Start()
    {
        var networkRunner = Instantiate(_networkRunnerPrefab);

        DontDestroyOnLoad(networkRunner.gameObject);

        // 現在のシーンを取得（シーン同期を無効化するため、各クライアントが独自のシーンを維持できる）
        var currentScene = SceneManager.GetActiveScene();
        var buildIndex = SceneUtility.GetBuildIndexByScenePath(currentScene.path);
        
        if (buildIndex < 0)
        {
            Debug.LogError($"Scene '{currentScene.path}' is not in Build Settings. Please add it to Build Settings.");
            return;
        }
        
        var currentSceneRef = SceneRef.FromIndex(buildIndex);

        // SceneManagerをnullにすることでシーン同期を無効化
        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            IsVisible = false,
            SessionName = "TestSession",
            PlayerCount = 4,
            Scene = currentSceneRef, // 現在のシーンを参照（シーン同期なし）
            SceneManager = null,     // シーン同期を無効化
        });

        Debug.Log($"StartGame Result: {result}");
    }
}
