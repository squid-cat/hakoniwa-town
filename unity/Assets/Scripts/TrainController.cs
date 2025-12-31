using UnityEngine;
using UnityEngine.UI;

public class TrainController : MonoBehaviour
{
    // �d�Ԃ̕����p�����[�^
    [SerializeField] private TrainNetworkState _trainNetworkState;

    [SerializeField] private Slider _notchSlider;

    private void Start()
    {
        if (_notchSlider != null)
        {
            _notchSlider.onValueChanged.AddListener(SetNotch);
        }
    }

    void SetNotch(float notch)
    {
        if (_trainNetworkState != null)
        {
            _trainNetworkState.CurrentNotch = Mathf.RoundToInt(notch);
        }
    }
}
