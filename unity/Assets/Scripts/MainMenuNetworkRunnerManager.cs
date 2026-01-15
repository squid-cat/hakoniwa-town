using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenuNetworkRunnerManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunner _networkRunnerPrefab;
    [SerializeField] private string targetSceneName;

    [SerializeField] private TMP_InputField _sessionNameInputField;
    [SerializeField] private TMP_Text _infoText;
    [SerializeField] private List<Button> _joinButtons = new List<Button>();

    private bool _isProcessing = false;

    private void Start()
    {
        _infoText.text = "";
    }

    public async void JoinMainGame()
    {
        if (_isProcessing) return;

        OnChangeProcessing(true);
        _infoText.text = "部屋を検索または作成中...";

        try
        {
            var sessionName = "";

            string inputSessionName = _sessionNameInputField.text;
            Debug.Log($"[JoinMainGame] Input Session Name: '{inputSessionName}'");
            if (string.IsNullOrEmpty(inputSessionName))
            {
                sessionName = Random.Range(10000000, 99999999).ToString();
            } else
            {
                sessionName = inputSessionName;
            }

                Debug.Log($"[JoinMainGame] Session Name: '{sessionName}'");

            await StartGameProcess(GameMode.Shared, sessionName, true);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[JoinMainGame] {e.Message}");
            _infoText.text = "エラーが発生しました";
        }
        finally
        {
            OnChangeProcessing(false);
        }
    }

    private void OnChangeProcessing(bool isProcessing)
    {
        _isProcessing = isProcessing;
        foreach (var btn in _joinButtons)
        {
            if (btn != null) btn.interactable = !isProcessing;
        }
    }

    private async Task StartGameProcess(GameMode mode, string sessionName, bool isVisible)
    {
        var runner = Instantiate(_networkRunnerPrefab);
        DontDestroyOnLoad(runner.gameObject);
        runner.AddCallbacks(this);

        var sceneManager = runner.GetComponent<NetworkSceneManagerDefault>();
        if (sceneManager == null) sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = sessionName,
            IsVisible = isVisible,
            PlayerCount = 4,
            Scene = SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath(targetSceneName)),
            SceneManager = sceneManager
        });

        if (!result.Ok)
        {
            _infoText.text = $"接続失敗: {result.ShutdownReason}";
            if (runner != null) Destroy(runner.gameObject);
        }
    }

    // --- インターフェースの空実装（省略せずに保持してください） ---
    #region INetworkRunnerCallbacks
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    #endregion
}
