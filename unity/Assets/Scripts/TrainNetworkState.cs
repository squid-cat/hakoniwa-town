using Fusion;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

public class TrainNetworkState : NetworkBehaviour
{
    [Networked]
    public int CurrentNotch { get; set; } = 0;
    [Networked] public float CurrentSpeed { get; set; } = 0f;

    [SerializeField] private SplineAnimate _splineAnimate;
    [SerializeField] private SplineContainer splineContainer;

    [SerializeField] private float _accelerationUnit = 2.0f; // ノッチ1あたりの加速度
    [SerializeField] private float _friction = 0.1f;        // 摩擦係数
    [SerializeField] private float _maxSpeed = 20f;         // 最高速度

    [SerializeField] CameraController _cameraController;
    [SerializeField] Slider _notchSlider;

    [SerializeField] private List<TextMeshProUGUI> _TrainInfoTextList = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> _TrainDebugInfoTextList = new List<TextMeshProUGUI>();

    // 全長の距離
    private float _totalSplineLength = 0f;
    // 残り距離を計算するための変数
    private float _remainingDistance = 0f;

    private bool _isUpdatingSliderFromNetwork = false;
    private int _previousNotch = 0; // 前回の値を保存

    // 残り距離に掛ける係数
    // NOTE: スピードと距離の単位が違うため、係数を掛けておおよその寸法を揃えるための係数
    private static readonly float DISTANCE_EPSILON = 4f;
    
    public override void Spawned()
    {
        // Sliderの値変更イベントにリスナーを登録
        if (_notchSlider != null)
        {
            _notchSlider.onValueChanged.AddListener(OnNotchSliderValueChanged);
            _notchSlider.value = CurrentNotch; // 初期値を設定
        }

        // Runnerを渡してカメラ設定を適用
        if (_cameraController != null && Runner != null)
        {
            _cameraController.ApplyInitialCameraSetup(Runner);
        }
        else if (_cameraController != null)
        {
            // Runnerがまだ設定されていない場合は、Runnerなしで試行
            _cameraController.ApplyInitialCameraSetup();
        }

        // スプラインの全長を計算
        if (splineContainer != null)
        {
            _totalSplineLength = splineContainer.CalculateLength();
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
        if (newNotch == CurrentNotch)
        {
            return;
        }
        
        // Sliderの値とCurrentNotchの差が大きい場合のみ更新（誤差を許容）
        if (Mathf.Abs(value - CurrentNotch) < 0.5f)
        {
            return;
        }
        
        Debug.Log($"[TrainNetworkState] Slider value changed: {value} -> {newNotch}, CurrentNotch: {CurrentNotch}");
        
        // 共有モードでは、すべてのクライアントが更新できる
        if (Object != null && Object.IsValid && Runner != null && Runner.IsRunning)
        {
            // RPCを使用して明示的に同期
            RPC_SetNotch(newNotch);
        }
    }

    // RPCを使用してnotchを同期
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SetNotch(int notch)
    {
        CurrentNotch = notch;
        _previousNotch = CurrentNotch;
        Debug.Log($"[TrainNetworkState] CurrentNotch updated via RPC to: {CurrentNotch}");
        
        // Sliderの更新はUpdateメソッドで行う（RPC内では更新しない）
    }

    private void OnDestroy()
    {
        // イベントリスナーを解除
        if (_notchSlider != null)
        {
            _notchSlider.onValueChanged.RemoveListener(OnNotchSliderValueChanged);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (_splineAnimate == null || !Object || !Object.IsValid) return;

        // 1. 加速度の計算
        // ノッチ正: 加速, ノッチ負: ブレーキ
        float acceleration = CurrentNotch * _accelerationUnit;

        // 2. 速度の更新 (V = V + a*t)
        CurrentSpeed += acceleration * Time.deltaTime;

        // 3. 摩擦と減速処理
        if (CurrentNotch == 0)
        {
            // 惰性走行（減速する）
            CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, 0, _friction * Time.deltaTime);
        }

        // 4. 速度の制限（負の値や最高速度を超えない）
        CurrentSpeed = Mathf.Max(0, CurrentSpeed); // 負の値にならない
        CurrentSpeed = Mathf.Clamp(CurrentSpeed, 0, _maxSpeed); // 最高速度を超えない

        // 5. 現在の位置を保存
        var previousPosition = _splineAnimate.NormalizedTime;

        // 6. SplineAnimateへの反映
        if (CurrentSpeed <= 0.01f)
        {
            if (_splineAnimate.IsPlaying) _splineAnimate.Pause();
        }
        else
        {
            if (!_splineAnimate.IsPlaying) _splineAnimate.Play();
            _splineAnimate.MaxSpeed = CurrentSpeed;   
        }

        // 7. 現在の位置を反映
        _splineAnimate.NormalizedTime = previousPosition; // 位置は変えず、速度だけを更新


        // 8. 現在のスプライン上の「終端までの残り弧長」（目安）
        UpdateRemainingDistanceAlongSpline();
    }

    /// <summary>
    /// 残り距離 ≒ 全弧長 × (1 - 進捗)。
    /// <see cref="SplineAnimate.NormalizedTime"/> の小数部が現在ループ内の進捗 (0–1)（公式ドキュメント準拠）。
    /// </summary>
    private void UpdateRemainingDistanceAlongSpline()
    {
        SplineContainer container = _splineAnimate != null && _splineAnimate.Container != null
            ? _splineAnimate.Container
            : splineContainer;

        if (container == null || container.Splines.Count == 0)
        {
            _remainingDistance = 0f;
            return;
        }

        // 1. スプラインの全延長を計算
        float totalLength = _totalSplineLength;

        // 2. 現在の位置を取得
        float t = _splineAnimate.NormalizedTime;
        
        // 3. 走行済みの距離
        float traveledDistance = t * totalLength;

        // 4. 残り距離
        float remainingDistance = totalLength - traveledDistance;

        _remainingDistance = remainingDistance * DISTANCE_EPSILON;
    }

    void Update()
    {
        // デバッグ情報を追加
        string debugInfo = "";
        if (Runner == null)
        {
            debugInfo = "Runner: null";
        }
        else if (!Runner.IsRunning)
        {
            debugInfo = $"Runner: {Runner.State}";
        }
        else if (!Object || !Object.IsValid)
        {
            debugInfo = $"Runner: Running, Object: {(Object != null ? Object.IsValid.ToString() : "null")}";
        }

        // 1. Runnerがなく、またはまだ起動（Running）していない場合
        if (Runner == null || !Runner.IsRunning)
        {
            SetTrainInfoText($"Connecting to Network...");
            SetTrainDebugInfoText($"{debugInfo}");
            return;
        }

        // 2. 接続はできているが、このオブジェクトがまだネットワークに存在しない場合
        if (!Object || !Object.IsValid)
        {
            SetTrainInfoText($"Spawning Train...");
            SetTrainDebugInfoText($"{debugInfo}");
            return;
        }

        // 3. ネットワークから受信した値でSliderを更新
        if (_notchSlider != null && _previousNotch != CurrentNotch)
        {
            int targetNotch = CurrentNotch;
            float currentSliderValue = _notchSlider.value;
            
            // Sliderの値とCurrentNotchの差が大きい場合のみ更新（振動を防ぐため閾値を大きく）
            if (Mathf.Abs(currentSliderValue - targetNotch) > 3f)
            {
                _isUpdatingSliderFromNetwork = true;
                _notchSlider.value = targetNotch;
                _isUpdatingSliderFromNetwork = false;
                Debug.Log($"[TrainNetworkState] CurrentNotch changed to: {CurrentNotch} (network sync), Slider updated from {currentSliderValue:F2} to {targetNotch}");
            }
            _previousNotch = CurrentNotch;
        }

        // 4. 通常の情報表示
        SetTrainInfoText(
            $"Notch  : {CurrentNotch}\n" +
            $"Speed : {CurrentSpeed:F2} cm/s\n" +
            $"Remaining Distance : {_remainingDistance:F2} cm"
        );
        SetTrainDebugInfoText(
            "-- Train Info --\n" +
            $"Acceleration: {_accelerationUnit:F1}\n" +
            $"Friction: {_friction:F1}\n" +
            $"MaxSpeed: {_maxSpeed:F1}\n" +
            "\n" +
            "-- Session Info --\n" +
            $"Name: {Runner.SessionInfo.Name}\n" +
            $"Players: {Runner.SessionInfo.PlayerCount}/{Runner.SessionInfo.MaxPlayers}\n" +
            $"Region: {Runner.SessionInfo.Region}\n" +
            $"YourPlayerId: {Runner.GetPlayerUserId()}"
            );
    }

    private void SetTrainInfoText(string displayText)
    {
        foreach (var text in _TrainInfoTextList)
        {
            text.text = displayText;
        }
    }

    private void SetTrainDebugInfoText(string debugText)
    {
        foreach (var text in _TrainDebugInfoTextList)
        {
            text.text = debugText;
        }
    }
}
