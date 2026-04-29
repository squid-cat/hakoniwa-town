using UnityEngine;
using UnityEngine.Splines;

public class MainGameStartSwitcher : MonoBehaviour
{
    [SerializeField] private SplineContainer targetContainer;
    [SerializeField] private SplineAnimate splineAnimate;

    void Start()
    {
        if (splineAnimate != null)
        {
            if (targetContainer != null)
            {
                splineAnimate.Container = targetContainer;
            }

            splineAnimate.Alignment = SplineAnimate.AlignmentMode.SplineElement;
        }
    }
}
