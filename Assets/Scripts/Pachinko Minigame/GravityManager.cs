using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class GravityManager : MonoBehaviour
{
    [SerializeField, Range(-1, 0)] private float maxLeftInclination = -0.7f;
    [SerializeField, Range(-1, 0)] private float maxRightInclination = -0.3f;
    private Vector2 initial_gravity;
    private void Awake()
    {
        if (!SystemInfo.supportsGyroscope)
        {
            throw new System.Exception("Tu dispositivo no tiene giroscopio");
        }

        GetComponent<PlayerInput>().actions["Gyro"].Enable();

        initial_gravity = new Vector2(0f, -9.81f);
    }

    private void Update()
    {
        if (GravitySensor.current != null)
        {
            InputSystem.EnableDevice(GravitySensor.current);
            OnGyro(GravitySensor.current.gravity.ReadValue());
        }
    }

    public void ResetGravity()
    {
        Physics2D.gravity = initial_gravity;
    }

    public void OnGyro(Vector3 gyroGravity)
    {
        Vector2 inputGravity = new Vector2(gyroGravity.x, gyroGravity.y + gyroGravity.z);
        inputGravity = new Vector2(inputGravity.x, Mathf.Clamp(inputGravity.y, maxLeftInclination, maxRightInclination)).normalized;
        Vector2 newGravity = inputGravity * Mathf.Abs(initial_gravity.y);
        Physics2D.gravity = newGravity;
    }
}
