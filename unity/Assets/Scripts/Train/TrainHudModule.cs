using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 電車のノッチ Slider・情報テキスト・デバッグ表示（TrainNetworkState から切り出し）。
/// </summary>
public sealed class TrainHudModule
{
    private readonly TrainNetworkState _train;
    private readonly TrainSimulationModule _simulation;
    private readonly Slider _notchSlider;
    private readonly List<TextMeshProUGUI> _trainInfoTextList;
    private readonly List<TextMeshProUGUI> _trainDebugInfoTextList;

    private readonly float _accelerationUnit;
    private readonly float _friction;
    private readonly float _maxSpeed;

    private bool _isUpdatingSliderFromNetwork;
    private int _previousNotch;

    public TrainHudModule(
        TrainNetworkState train,
        TrainSimulationModule simulation,
        Slider notchSlider,
        List<TextMeshProUGUI> trainInfoTextList,
        List<TextMeshProUGUI> trainDebugInfoTextList,
        float accelerationUnit,
        float friction,
        float maxSpeed)
    {
        _train = train;
        _simulation = simulation;
        _notchSlider = notchSlider;
        _trainInfoTextList = trainInfoTextList;
        _trainDebugInfoTextList = trainDebugInfoTextList;
        _accelerationUnit = accelerationUnit;
        _friction = friction;
        _maxSpeed = maxSpeed;
    }

    public void OnSpawnedInitialSlider(int currentNotch)
    {
        if (_notchSlider != null)
        {
            _notchSlider.onValueChanged.AddListener(OnNotchSliderValueChanged);
            _notchSlider.value = currentNotch; // 初期値を設定
        }
    }

    public void OnDestroyUnregisterSlider()
    {
        if (_notchSlider != null)
        {
            _notchSlider.onValueChanged.RemoveListener(OnNotchSliderValueChanged);
        }
    }

    private void OnNotchSliderValueChanged(float value)
    {
        // ネットワークからSliderを更新している場合は無視（無限ループを防ぐ）
        if (_isUpdatingSliderFromNetwork)
        {
            return;
        }

        // Sliderの値を整数に丸める
        int newNotch = Mathf.RoundToInt(value);

        // 現在の値と同じ、または差が小さい場合は無視（振動を防ぐ）
        if (newNotch == _train.CurrentNotch)
        {
            return;
        }

        // Sliderの値とCurrentNotchの差が大きい場合のみ更新（誤差を許容）
        if (Mathf.Abs(value - _train.CurrentNotch) < 0.5f)
        {
            return;
        }

        Debug.Log($"[TrainNetworkState] Slider value changed: {value} -> {newNotch}, CurrentNotch: {_train.CurrentNotch}");

        // 共有モードでは、すべてのクライアントが更新できる
        if (_train.Object != null && _train.Object.IsValid && _train.Runner != null && _train.Runner.IsRunning)
        {
            // RPCを使用して明示的に同期
            _train.RequestSetNotchFromHud(newNotch);
        }
    }

    public void SyncPreviousNotchAfterRpc(int notch)
    {
        _previousNotch = notch;
    }

    public void UpdateFrame()
    {
        NetworkRunner runner = _train.Runner;

        // デバッグ情報を追加
        string debugInfo = "";
        if (runner == null)
        {
            debugInfo = "Runner: null";
        }
        else if (!runner.IsRunning)
        {
            debugInfo = $"Runner: {runner.State}";
        }
        else if (!_train.Object || !_train.Object.IsValid)
        {
            debugInfo = $"Runner: Running, Object: {(_train.Object != null ? _train.Object.IsValid.ToString() : "null")}";
        }

        // 1. Runnerがなく、またはまだ起動（Running）していない場合
        if (runner == null || !runner.IsRunning)
        {
            SetTrainInfoText($"Connecting to Network...");
            SetTrainDebugInfoText($"{debugInfo}");
            return;
        }

        // 2. 接続はできているが、このオブジェクトがまだネットワークに存在しない場合
        if (!_train.Object || !_train.Object.IsValid)
        {
            SetTrainInfoText($"Spawning Train...");
            SetTrainDebugInfoText($"{debugInfo}");
            return;
        }

        // 3. ネットワークから受信した値でSliderを更新
        if (_notchSlider != null && _previousNotch != _train.CurrentNotch)
        {
            int targetNotch = _train.CurrentNotch;
            float currentSliderValue = _notchSlider.value;

            // Sliderの値とCurrentNotchの差が大きい場合のみ更新（振動を防ぐため閾値を大きく）
            if (Mathf.Abs(currentSliderValue - targetNotch) > 3f)
            {
                _isUpdatingSliderFromNetwork = true;
                _notchSlider.value = targetNotch;
                _isUpdatingSliderFromNetwork = false;
                Debug.Log($"[TrainNetworkState] CurrentNotch changed to: {_train.CurrentNotch} (network sync), Slider updated from {currentSliderValue:F2} to {targetNotch}");
            }
            _previousNotch = _train.CurrentNotch;
        }

        // 4. 通常の情報表示
        SetTrainInfoText(
            $"Notch  : {_train.CurrentNotch}\n" +
            $"Speed : {_train.CurrentSpeed:F2} cm/s\n" +
            $"Remaining Distance : {_simulation.RemainingDistance:F2} cm"
        );
        SetTrainDebugInfoText(
            "-- Train Info --\n" +
            $"Acceleration: {_accelerationUnit:F1}\n" +
            $"Friction: {_friction:F1}\n" +
            $"MaxSpeed: {_maxSpeed:F1}\n" +
            "\n" +
            "-- Session Info --\n" +
            $"Name: {runner.SessionInfo.Name}\n" +
            $"Players: {runner.SessionInfo.PlayerCount}/{runner.SessionInfo.MaxPlayers}\n" +
            $"Region: {runner.SessionInfo.Region}\n" +
            $"YourPlayerId: {runner.GetPlayerUserId()}"
            );
    }

    private void SetTrainInfoText(string displayText)
    {
        foreach (var text in _trainInfoTextList)
        {
            if (text != null)
            {
                text.text = displayText;
            }
        }
    }

    private void SetTrainDebugInfoText(string debugText)
    {
        foreach (var text in _trainDebugInfoTextList)
        {
            if (text != null)
            {
                text.text = debugText;
            }
        }
    }
}
