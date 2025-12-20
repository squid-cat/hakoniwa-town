using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControll : MonoBehaviour
{
    [SerializeField] private GameObject _mainCamera;
    [SerializeField] private GameObject _subCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mainCamera.SetActive(true);
        _subCamera.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        var current = Keyboard.current;
        var spaceKey = current.spaceKey;

        if (spaceKey.wasPressedThisFrame)
        {
            changeCamera();
        }
    }

    public void OnChangeCamera()
    {
       changeCamera();
    }

    private void changeCamera()
    {
        bool isMainActive = _mainCamera.activeSelf;
        _mainCamera.SetActive(!isMainActive);
        _subCamera.SetActive(isMainActive);
    }
}
