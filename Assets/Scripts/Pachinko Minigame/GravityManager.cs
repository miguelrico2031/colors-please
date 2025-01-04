using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class GravityManager : MonoBehaviour
{
    private Vector2 initial_gravity;
    private void Awake()
    {
        if (!SystemInfo.supportsGyroscope)
        {
            throw new System.Exception("Tu dispositivo no tiene giroscopio");
        }

        if (GravitySensor.current != null)
        {
            InputSystem.EnableDevice(GravitySensor.current);
        }
        
        GetComponent<PlayerInput>().actions["Gyro"].Enable();

        initial_gravity = Physics2D.gravity;
    }

    private void Update()
    {
        Debug.Log(GravitySensor.current.gravity.ReadValue());
    }

    private void ResetGravity()
    {
        Physics2D.gravity = initial_gravity;
    }

    private void OnGyro(InputAction.CallbackContext context)
    {
        Vector3 gyroGravity = context.ReadValue<Vector3>();
        Vector2 inputGravity = new Vector2(gyroGravity.x, gyroGravity.y + gyroGravity.z);
        inputGravity = new Vector2(inputGravity.x, Mathf.Clamp(inputGravity.y, -1.0f, 0f)).normalized;
        Vector2 newGravity = inputGravity * Mathf.Abs(initial_gravity.y);
        Physics2D.gravity = newGravity;
    }
}
