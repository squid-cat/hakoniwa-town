using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// 電車のスプライン上の速度・残り距離などのシミュレーション（TrainNetworkState から切り出し）。
/// </summary>
public sealed class TrainSimulationModule
{
    private readonly SplineAnimate _splineAnimate;
    private readonly SplineContainer _splineContainer;

    private readonly float _accelerationUnit;
    private readonly float _friction;
    private readonly float _maxSpeed;

    private float _totalSplineLength;
    private float _remainingDistance;

    // 残り距離に掛ける係数
    // NOTE: スピードと距離の単位が違うため、係数を掛けておおよその寸法を揃えるための係数
    private static readonly float DISTANCE_EPSILON = 4.2f;

    public float RemainingDistance => _remainingDistance;

    public TrainSimulationModule(
        SplineAnimate splineAnimate,
        SplineContainer splineContainer,
        float accelerationUnit,
        float friction,
        float maxSpeed)
    {
        _splineAnimate = splineAnimate;
        _splineContainer = splineContainer;
        _accelerationUnit = accelerationUnit;
        _friction = friction;
        _maxSpeed = maxSpeed;

        if (_splineAnimate != null)
        {
            _splineAnimate.Alignment = SplineAnimate.AlignmentMode.SplineElement;
        }
    }

    public void CacheTotalSplineLength()
    {
        if (_splineContainer != null)
        {
            _totalSplineLength = _splineContainer.CalculateLength();
        }
    }

    public void FixedUpdateStep(ref float currentSpeed, int currentNotch, float deltaTime)
    {
        if (_splineAnimate == null)
        {
            return;
        }

        // 1. 加速度の計算
        // ノッチ正: 加速, ノッチ負: ブレーキ
        float acceleration = currentNotch * _accelerationUnit;

        // 2. 速度の更新 (V = V + a*t)
        currentSpeed += acceleration * deltaTime;

        // 3. 摩擦と減速処理
        if (currentNotch == 0)
        {
            // 惰性走行（減速する）
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, _friction * deltaTime);
        }

        // 4. 速度の制限（負の値や最高速度を超えない）
        currentSpeed = Mathf.Max(0, currentSpeed); // 負の値にならない
        currentSpeed = Mathf.Clamp(currentSpeed, 0, _maxSpeed); // 最高速度を超えない

        // 5. 現在の位置を保存
        var previousPosition = _splineAnimate.NormalizedTime;

        // 6. SplineAnimateへの反映
        if (currentSpeed <= 0.01f)
        {
            if (_splineAnimate.IsPlaying) _splineAnimate.Pause();
        }
        else
        {
            if (!_splineAnimate.IsPlaying) _splineAnimate.Play();
            _splineAnimate.MaxSpeed = currentSpeed;
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
            : _splineContainer;

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
}
