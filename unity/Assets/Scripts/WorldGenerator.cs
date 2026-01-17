using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private GameObject stationPrefab;
    [SerializeField] private GameObject plane;
    [SerializeField] private SplineInstantiate[] splineInstantiates;

    void Start()
    {
        // 駅格納用コンテナ
        GameObject stationContainer = new GameObject("StationContainer");

        // Unity Editor で最初から存在するデフォルトのスプラインを削除
        Spline defaultSpline = splineContainer.Splines[0];
        splineContainer.RemoveSpline(defaultSpline);

        Vector3 currentPosition = new Vector3(0, 0, 0);
        Vector3 acceleration = new Vector3(1, 0, 0);

        for (int i = 0; i < 5; i++)
        {
            Spline spline = splineContainer.AddSpline();

            spline.Add(new BezierKnot(currentPosition));

            // 最初にレールをまっすぐ伸ばす
            float x_before_rate = CalcRate(acceleration.x, acceleration.z, 1f);
            float z_before_rate = CalcRate(acceleration.z, acceleration.x, 0f);

            currentPosition.x += x_before_rate * 10f;
            currentPosition.z += z_before_rate * 10f;

            spline.Add(new BezierKnot(currentPosition));

            for (int j = 0; j < 50; j++) {
                acceleration.x = Mathf.Clamp(acceleration.x + UnityEngine.Random.Range(-0.1f, 0.1f), 0f, 1f);
                acceleration.z = Mathf.Clamp(acceleration.z + UnityEngine.Random.Range(-0.1f, 0.1f), -1f, 1f);

                float x_rate = CalcRate(acceleration.x, acceleration.z, 1f);
                float z_rate = CalcRate(acceleration.z, acceleration.x, 0f);

                currentPosition.x += x_rate * 5f;
                currentPosition.z += z_rate * 5f;

                spline.Add(new BezierKnot(currentPosition));
            }

            // 最後にレールをまっすぐ伸ばす
            float x_after_rate = CalcRate(acceleration.x, acceleration.z, 1f);
            float z_after_rate = CalcRate(acceleration.z, acceleration.x, 0f);

            currentPosition.x += x_after_rate * 10f;
            currentPosition.z += z_after_rate * 10f;

            spline.Add(new BezierKnot(currentPosition));

            // 角度を滑らかに
            spline.SetTangentMode(TangentMode.AutoSmooth);

            // 終点に駅を設置
            float offsetDistance = 2f;

            BezierKnot lastKnot = spline.Knots.Last();
            Vector3 knotPosition = lastKnot.Position;
            Quaternion knotRotation = lastKnot.Rotation;

            Vector3 knotLeftRotation = ((Quaternion) lastKnot.Rotation) * Vector3.left;

            Vector3 stationPosition = knotPosition + (knotLeftRotation * offsetDistance);
            stationPosition.y = 0;

            Quaternion stationRotation = Quaternion.Euler(
                0,
                knotRotation.eulerAngles.y,
                0
            );

            GameObject station = Instantiate(stationPrefab, stationPosition, stationRotation);
            station.transform.SetParent(stationContainer.transform);
        }

        // SplineInstantiate を遅延更新（ビルド後はAutoRefreshが機能しないため）
        StartCoroutine(UpdateSplineInstantiatesDelayed());
    }

    /// <summary>
    /// SplineInstantiate を遅延更新する（スプライン生成完了後に実行）
    /// </summary>
    private IEnumerator UpdateSplineInstantiatesDelayed()
    {
        // 1フレーム待つ（スプラインの内部状態が更新されるのを待つ）
        yield return null;
        
        if (splineInstantiates != null)
        {
            foreach (var splineInstantiate in splineInstantiates)
            {
                if (splineInstantiate != null)
                {
                    splineInstantiate.UpdateInstances();
                }
            }
        }
    }

    private float CalcRate(float target, float other, float initial)
    {
        float total_acc = Mathf.Abs(target) + Mathf.Abs(other);

        if (total_acc == 0)
        {
            return initial;
        }

        return target / total_acc;
    }
}
