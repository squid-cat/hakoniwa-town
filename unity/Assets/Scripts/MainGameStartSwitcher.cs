using UnityEngine;
using UnityEngine.Splines;

public class MainGameStartSwitcher : MonoBehaviour
{
    [SerializeField] private SplineContainer targetContainer;
    [SerializeField] private SplineAnimate splineAnimate;

    void Start()
    {
        if (splineAnimate != null && targetContainer != null)
        {
            splineAnimate.Container = targetContainer;
        }
    }
}
