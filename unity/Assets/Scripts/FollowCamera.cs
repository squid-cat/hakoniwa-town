using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Offset")]
    [SerializeField] private Vector3 positionOffset = new Vector3(0, 5, -10);
    [SerializeField] private bool useLocalOffset = true;

    [Header("Smoothing")]
    [SerializeField] private float positionSmoothTime = 0.3f;
    [SerializeField] private float rotationSmoothTime = 0.1f;

    [Header("Look At")]
    [SerializeField] private bool lookAtTarget = true;
    [SerializeField] private Vector3 lookAtOffset = Vector3.zero;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate target position
        Vector3 targetPosition;
        if (useLocalOffset)
        {
            // Offset relative to target's orientation
            targetPosition = target.position + target.TransformDirection(positionOffset);
        }
        else
        {
            // Fixed world offset
            targetPosition = target.position + positionOffset;
        }

        // Smooth position
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            targetPosition, 
            ref velocity, 
            positionSmoothTime
        );

        // Look at target
        if (lookAtTarget)
        {
            Vector3 lookAtPosition = target.position + lookAtOffset;
            Quaternion targetRotation = Quaternion.LookRotation(lookAtPosition - transform.position);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                Time.deltaTime / rotationSmoothTime
            );
        }
    }
}
