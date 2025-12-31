using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;

public class TrainNetworkState : NetworkBehaviour
{
    [Networked] public int CurrentNotch { get; set; }
    [Networked] public float CurrentSpeed { get; set; }

    [SerializeField] private SplineAnimate _splineAnimate;
    [SerializeField] private float _accelerationUnit = 2.0f; // ノッチ1あたりの加速度
    [SerializeField] private float _friction = 0.1f;        // 自然減速
    [SerializeField] private float _maxSpeed = 20f;         // 最高速度

    [SerializeField] private TextMeshProUGUI _TrainInfoText;

    public override void Spawned()
    {
        CurrentNotch = 0;
        CurrentSpeed = 0f;
    }

    public override void FixedUpdateNetwork()
    {
        if (_splineAnimate == null || !Object || !Object.IsValid) return;

        // 1. 加速度の計算
        // ノッチ正: 加速, ノッチ負: ブレーキ
        float acceleration = CurrentNotch * _accelerationUnit;

        // 2. 速度の更新 (V = V + a*t)
        CurrentSpeed += acceleration * Time.deltaTime;

        // 3. 自然減速と停止処理
        if (CurrentNotch == 0)
        {
            // 惰性走行（少しずつ減速）
            CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, 0, _friction * Time.deltaTime);
        }

        // 4. 速度のクランプ（逆走防止や最高速制限）
        CurrentSpeed = Mathf.Max(0, CurrentSpeed); // バックさせない場合
        CurrentSpeed = Mathf.Clamp(CurrentSpeed, 0, _maxSpeed); // 最高時速を超過しない

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

        // 8. デバッグ情報の反映
        if (_TrainInfoText != null)
        {
            _TrainInfoText.text =
                $"Notch  : {CurrentNotch}\n" +
                $"Speed : {CurrentSpeed:F1} m/s";
        }
    }

    void Update()
    {
        if (_TrainInfoText == null) return;

        // 1. Runnerがない、もしくはまだ起動（Running）していない場合
        if (Runner == null || !Runner.IsRunning)
        {
            _TrainInfoText.text = "Connecting to Network...";
            return;
        }

        // 2. 接続はされているが、このオブジェクトがまだネットワークに存在しない場合
        if (!Object || !Object.IsValid)
        {
            _TrainInfoText.text = "Spawning Train...";
        }
    }

}
