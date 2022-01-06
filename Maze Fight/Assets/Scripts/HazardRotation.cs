using UnityEngine;

public class HazardRotation : MonoBehaviour
{
    [Header("Standard Rotation")]
    public Transform RotationPoint;
    public float RotationSpeedX = 0f;
    public float RotationSpeedY = 0f;
    public float RotationSpeedZ = 0f;

    [Space]
    [Header("Oscillation Settings")]
    public bool IsOscillatorX = false;
    public float MinAngleX = 0f;
    public float MaxAngleX = 360f;
    public float OscillationSpeedX = 0f;
    public bool IsOscillatorY = false;
    public float MinAngleY = 0f;
    public float MaxAngleY = 360f;
    public float OscillationSpeedY = 0f;
    public bool IsOscillatorZ = false;
    public float MinAngleZ = 0f;
    public float MaxAngleZ = 360f;
    public float OscillationSpeedZ = 0f;

    void Start()
    {
        if (!RotationPoint)
        {
            RotationPoint = transform;
        }
        //InitAngle();
    }
    
    void Update()
    {
        UpdateRotation();
    }

    void InitAngle()
    {
        Vector3 startRot = Vector3.zero;

        if (IsOscillatorX)
            startRot.x = MinAngleX + 0.1f;
        if (IsOscillatorY)
            startRot.y = MinAngleY + 0.1f;
        if (IsOscillatorZ)
            startRot.z = MinAngleZ + 0.1f;

        RotationPoint.Rotate(startRot);
    }

    void UpdateRotation()
    {
        if (IsOscillatorX || IsOscillatorY || IsOscillatorZ)
        {
            float rX = 0f, rY = 0f, rZ = 0f;

            if (IsOscillatorX)
            {
                rX = Mathf.SmoothStep(MinAngleX, MaxAngleX, Mathf.PingPong(Time.time * OscillationSpeedX, 1));
            }
            if (IsOscillatorY)
            {
                rY = Mathf.SmoothStep(MinAngleY, MaxAngleY, Mathf.PingPong(Time.time * OscillationSpeedY, 1));
            }
            if (IsOscillatorZ)
            {
                rZ = Mathf.SmoothStep(MinAngleZ, MaxAngleZ, Mathf.PingPong(Time.time * OscillationSpeedZ, 1));
            }
            transform.rotation = Quaternion.Euler(rX, rY, rZ);
        }
        else
        {
            RotationPoint.Rotate(new Vector3(RotationSpeedX, RotationSpeedY, RotationSpeedZ) * Time.deltaTime, Space.Self);
        }
    }
}
