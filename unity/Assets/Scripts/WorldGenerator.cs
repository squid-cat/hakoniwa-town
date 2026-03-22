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
        // �w�i�[�p�R���e�i
        GameObject stationContainer = new GameObject("StationContainer");

        // Unity Editor �ōŏ����瑶�݂���f�t�H���g�̃X�v���C�����폜
        Spline defaultSpline = splineContainer.Splines[0];
        splineContainer.RemoveSpline(defaultSpline);

        Vector3 currentPosition = new Vector3(0, 0, 0);
        Vector3 acceleration = new Vector3(1, 0, 0);

        for (int i = 0; i < 5; i++)
        {
            Spline spline = splineContainer.AddSpline();

            spline.Add(new BezierKnot(currentPosition));

            // �ŏ��Ƀ��[�����܂������L�΂�
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

            // �Ō�Ƀ��[�����܂������L�΂�
            float x_after_rate = CalcRate(acceleration.x, acceleration.z, 1f);
            float z_after_rate = CalcRate(acceleration.z, acceleration.x, 0f);

            currentPosition.x += x_after_rate * 10f;
            currentPosition.z += z_after_rate * 10f;

            spline.Add(new BezierKnot(currentPosition));

            // �p�x�����炩��
            spline.SetTangentMode(TangentMode.AutoSmooth);

            // �I�_�ɉw��ݒu
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

        // SplineInstantiate ��x���X�V�i�r���h���AutoRefresh���@�\���Ȃ����߁j
        StartCoroutine(UpdateSplineInstantiatesDelayed());
    }

    /// <summary>
    /// SplineInstantiate ��x���X�V����i�X�v���C������������Ɏ��s�j
    /// </summary>
    private IEnumerator UpdateSplineInstantiatesDelayed()
    {
        // 1�t���[���҂i�X�v���C���̓�����Ԃ��X�V�����̂�҂j
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
