using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkRunnerManager : MonoBehaviour
{
    [SerializeField]
    private NetworkRunner _networkRunnerPrefab;

    private async void Start()
    {
        // NetworkRunner のインスタンスを生成
        var networkRunner = Instantiate(_networkRunnerPrefab);
        // ゲームセッションを開始(共有モード)
        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            IsVisible = false,
            SessionName = "TestSession",
            PlayerCount = 4,
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = GetComponent<NetworkSceneManagerDefault>()
        });

        Debug.Log($"StartGame Result: {result}");
    }
}
