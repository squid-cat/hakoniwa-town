using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

public class TrainNetworkState : NetworkBehaviour
{
    [Networked] public int CurrentNotch { get; set; }
    [Networked] public float CurrentSpeed { get; set; }

    [SerializeField] private SplineAnimate _splineAnimate;
    [SerializeField] private float _accelerationUnit = 2.0f; // ノッチ1あたりの加速度
    [SerializeField] private float _friction = 0.1f;        // 摩擦係数
    [SerializeField] private float _maxSpeed = 20f;         // 最高速度

    [SerializeField] Slider _notchSlider;

    [SerializeField] private TextMeshProUGUI _TrainInfoText;

    public override void Spawned()
    {
        CurrentNotch = 0;
        CurrentSpeed = 0f;
    }

    public override void FixedUpdateNetwork()
    {
        if (_notchSlider != null)
        {
            _notchSlider.value = CurrentNotch;
        }

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
        _splineAnimate.NormalizedTime = previousPosition;
    }

    void Update()
    {
        if (_TrainInfoText == null) return;

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
            _TrainInfoText.text = $"Connecting to Network...\n{debugInfo}";
            return;
        }

        // 2. 接続はできているが、このオブジェクトがまだネットワークに存在しない場合
        if (!Object || !Object.IsValid)
        {
            _TrainInfoText.text = $"Spawning Train...\n{debugInfo}";
            return;
        }

        // 3. 通常の情報表示
        _TrainInfoText.text =
            $"Notch  : {CurrentNotch}\n" +
            $"Speed : {CurrentSpeed:F1} m/s";
    }

}
