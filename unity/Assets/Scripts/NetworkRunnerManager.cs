using Fusion;
using UnityEngine;

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
        });

        Debug.Log($"StartGame Result: {result}");
    }
}
