using UnityEngine;
using UnityEngine.Splines;

public class TrainControll : MonoBehaviour
{
    [SerializeField] private SplineAnimate _splineAnimate;

    public void SetTrainSpeed(float newSpeed)
    {
        // 現在の進行状況を取得
        float currentProgress = _splineAnimate.NormalizedTime;

        if (_splineAnimate == null) return;

        if (newSpeed <= 0f)
        {
            _splineAnimate.Pause();
        }
        else
        {
            if (!_splineAnimate.IsPlaying)
            {
                _splineAnimate.Play();
            }

            // SplineAnimate の MaxSpeed プロパティを更新
            _splineAnimate.MaxSpeed = newSpeed;

            // 進行状況を維持するために NormalizedTime を再設定
            _splineAnimate.NormalizedTime = currentProgress;
        }
    }
}
