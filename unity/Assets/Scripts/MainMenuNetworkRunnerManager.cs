using Fusion;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuNetworkRunnerManager : MonoBehaviour
{
    [SerializeField] private NetworkRunner _networkRunnerPrefab;
    [SerializeField] private string targetSceneName;

    public async void OnStartHost()
    {
        var networkRunner = Instantiate(_networkRunnerPrefab);

        DontDestroyOnLoad(networkRunner.gameObject);

        var sceneManager = networkRunner.GetComponent<NetworkSceneManagerDefault>();
        if (sceneManager == null)
        {
            sceneManager = networkRunner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        // 8Œ…‚Ìƒ‰ƒ“ƒ_ƒ€‚È”š
        string randomNumber = Random.Range(10000000, 99999999).ToString();

        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            IsVisible = false,
            SessionName = randomNumber, // •”‰®–¼‚ğƒ‰ƒ“ƒ_ƒ€‚É‚·‚é
            PlayerCount = 4,
            Scene = SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath(targetSceneName)),
            SceneManager = sceneManager
        });
    }
}
