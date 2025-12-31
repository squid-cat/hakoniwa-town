using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkRunnerManager : MonoBehaviour
{
    [SerializeField]
    private NetworkRunner _networkRunnerPrefab;

    private async void Start()
    {
        // NetworkRunner �̃C���X�^���X�𐶐�
        var networkRunner = Instantiate(_networkRunnerPrefab);

        DontDestroyOnLoad(networkRunner.gameObject);

        var sceneManager = networkRunner.GetComponent<NetworkSceneManagerDefault>();
        if (sceneManager == null)
        {
            sceneManager = networkRunner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        // �Q�[���Z�b�V�������J�n(���L���[�h)
        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            IsVisible = false,
            SessionName = "TestSession",
            PlayerCount = 4,
            Scene = SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath(SceneManager.GetActiveScene().path)),
            SceneManager = sceneManager
        });

        Debug.Log($"StartGame Result: {result}");
    }
}
