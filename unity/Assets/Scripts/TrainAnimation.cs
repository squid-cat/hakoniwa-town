using UnityEngine;

public class TrainAnimation : MonoBehaviour
{
    [Header("Network State")]
    [SerializeField] private TrainNetworkState trainNetworkState;

    [Header("Train Root")]
    [SerializeField] private Transform trainRoot;

    [Header("Wheels")]
    [SerializeField] private Transform[] wheelBones;
    [SerializeField] private float wheelRotationSensitivity = 180f;
    [SerializeField] private Vector3 wheelRotationAxis = Vector3.right;

    [Header("Crank Pin (on main drive wheel)")]
    [SerializeField] private Transform crankPinWheel;
    [SerializeField] private Vector3 crankPinLocalOffset = new Vector3(0, 0.1f, 0);

    [Header("Side Rods (follow crank pin position)")]
    [SerializeField] private Transform[] sideRodBones;

    [Header("Piston (front-back motion)")]
    [SerializeField] private Transform pistonBone;
    [SerializeField] private float pistonStroke = 0.1f;
    [SerializeField] private Vector3 pistonMotionAxis = Vector3.forward;

    // === Initial state storage ===
    
    // Side rods: initial position/rotation relative to trainRoot
    private Vector3[] sideRodInitialLocalPos;
    private Quaternion[] sideRodInitialLocalRot;
    
    // Piston: initial position/rotation relative to trainRoot
    private Vector3 pistonInitialLocalPos;
    private Quaternion pistonInitialLocalRot;

    private float currentWheelAngle = 0f;

    void Start()
    {
        if (trainRoot == null)
            trainRoot = transform;

        // Store side rod initial states
        if (sideRodBones != null && sideRodBones.Length > 0)
        {
            sideRodInitialLocalPos = new Vector3[sideRodBones.Length];
            sideRodInitialLocalRot = new Quaternion[sideRodBones.Length];

            for (int i = 0; i < sideRodBones.Length; i++)
            {
                if (sideRodBones[i] != null)
                {
                    sideRodInitialLocalPos[i] = trainRoot.InverseTransformPoint(sideRodBones[i].position);
                    sideRodInitialLocalRot[i] = Quaternion.Inverse(trainRoot.rotation) * sideRodBones[i].rotation;
                }
            }
        }

        // Store piston initial state
        if (pistonBone != null)
        {
            pistonInitialLocalPos = trainRoot.InverseTransformPoint(pistonBone.position);
            pistonInitialLocalRot = Quaternion.Inverse(trainRoot.rotation) * pistonBone.rotation;
        }
    }

    void Update()
    {
        if (trainNetworkState == null) return;
        if (!trainNetworkState.Object || !trainNetworkState.Object.IsValid) return;

        float speed = trainNetworkState.CurrentSpeed;
        float rotationDelta = speed * wheelRotationSensitivity * Time.deltaTime;
        currentWheelAngle += rotationDelta;
        float angleRad = currentWheelAngle * Mathf.Deg2Rad;

        // Calculate crank pin movement
        Vector3 crankPinMovement = CalculateCrankPinMovement();

        // 1. Rotate wheels
        RotateWheels(rotationDelta);

        // 2. Move side rods
        MoveSideRods(crankPinMovement);

        // 3. Move piston
        MovePiston(angleRad);
    }

    private Vector3 CalculateCrankPinMovement()
    {
        // Calculate crank pin movement based on wheel rotation
        Quaternion rotation = Quaternion.AngleAxis(currentWheelAngle, wheelRotationAxis);
        Vector3 rotatedOffset = rotation * crankPinLocalOffset;
        Vector3 localMovement = rotatedOffset - crankPinLocalOffset;
        return trainRoot.TransformDirection(localMovement);
    }

    private void RotateWheels(float rotationDelta)
    {
        if (wheelBones == null) return;

        Vector3 worldRotationAxis = trainRoot.TransformDirection(wheelRotationAxis);

        foreach (var wheel in wheelBones)
        {
            if (wheel != null)
            {
                wheel.Rotate(worldRotationAxis, rotationDelta, Space.World);
            }
        }
    }

    private void MoveSideRods(Vector3 crankPinMovement)
    {
        if (sideRodBones == null || sideRodInitialLocalPos == null) return;

        for (int i = 0; i < sideRodBones.Length; i++)
        {
            if (sideRodBones[i] != null)
            {
                // Position: initial position + crank pin movement
                Vector3 initialWorldPos = trainRoot.TransformPoint(sideRodInitialLocalPos[i]);
                sideRodBones[i].position = initialWorldPos + crankPinMovement;

                // Rotation: keep initial rotation
                sideRodBones[i].rotation = trainRoot.rotation * sideRodInitialLocalRot[i];
            }
        }
    }

    private void MovePiston(float angleRad)
    {
        if (pistonBone == null) return;

        // Position: initial position + oscillation
        Vector3 initialWorldPos = trainRoot.TransformPoint(pistonInitialLocalPos);
        Vector3 worldMotionAxis = trainRoot.TransformDirection(pistonMotionAxis);
        float offset = (Mathf.Cos(angleRad) - 1f) * pistonStroke * 0.5f;
        
        pistonBone.position = initialWorldPos + worldMotionAxis * offset;

        // Rotation: keep initial rotation
        pistonBone.rotation = trainRoot.rotation * pistonInitialLocalRot;
    }
}
