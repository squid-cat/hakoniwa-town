using Fusion;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject _mainCamera;
    [SerializeField] private GameObject _subCamera;
    [SerializeField] private GameObject _controllerCamera;

    private NetworkRunner _runner;

    void Start()
    {
        _mainCamera.SetActive(true);
        _subCamera.SetActive(false);
        _controllerCamera.SetActive(false);

        ApplyInitialCameraSetup();
    }

    public void ApplyInitialCameraSetup()
    {
        Debug.Log("Applying initial camera setup...");

        // _runnerがnullの場合は取得を試みる
        if (_runner == null)
        {
            _runner = FindFirstObjectByType<NetworkRunner>();
        }

        if (_runner == null || !_runner.IsRunning)
        {
            Debug.LogWarning("[CameraController] NetworkRunner is null or not running, cannot apply camera setup");
            return;
        }

        Debug.Log($"IsSharedModeMasterClient: {_runner.IsSharedModeMasterClient}");

        if (_runner.IsSharedModeMasterClient)
        {
            _mainCamera.SetActive(true);
            _subCamera.SetActive(false);
            _controllerCamera.SetActive(false);
        }
        else
        {
            _mainCamera.SetActive(false);
            _subCamera.SetActive(false);
            _controllerCamera.SetActive(true);
        }
    }

    public void ApplyInitialCameraSetup(NetworkRunner runner)
    {
        if (runner == null || !runner.IsRunning)
        {
            Debug.LogWarning("[CameraController] NetworkRunner is null or not running, cannot apply camera setup");
            return;
        }

        _runner = runner;
        ApplyInitialCameraSetup();
    }

    public void OnChangeMainCamera()
    {
        changeMainCamera();
    }

    private void changeMainCamera()
    {
        bool isMainActive = _mainCamera.activeSelf;
        _mainCamera.SetActive(!isMainActive);
        _subCamera.SetActive(isMainActive);
    }
}
