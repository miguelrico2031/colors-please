using UnityEngine;
using UnityEngine.InputSystem;

public class PaintGravityManager : MonoBehaviour
{
    [SerializeField] private Vector3 init_gravity;
    private bool endedGame = false;

    private void Awake()
    {
        init_gravity = Physics.gravity;

        endedGame = false;

        /*if (SystemInfo.supportsGyroscope)
        {
            //Input.gyro.enabled = true;
        }
        else
        {
            Debug.LogWarning("El giroscopio no está disponible en este dispositivo.");
        }*/

        if (!SystemInfo.supportsGyroscope)
        {
            throw new System.Exception("Tu dispositivo no tiene giroscopio");
        }

        GetComponent<PlayerInput>().actions["Gyro"].Enable();
    }

    void Update()
    {
        if (endedGame) return;

        if (GravitySensor.current != null)
        {
            InputSystem.EnableDevice(GravitySensor.current);
            OnGyro(GravitySensor.current.gravity.ReadValue());
        }
    }

    private void OnGyro(Vector3 gyroGravity)
    {
        Vector2 inputGravity = new Vector2(gyroGravity.x, gyroGravity.y + gyroGravity.z);
        inputGravity = new Vector2(inputGravity.x, Mathf.Clamp(inputGravity.y, -1.0f, 0f)).normalized;
        Vector2 newGravity = inputGravity * Mathf.Abs(init_gravity.y) * 2.5f;
        Physics2D.gravity = newGravity;
        Debug.Log("Modificando gravedad");
    }

    public void ResetGravity()
    {
        Debug.Log("Reiniciando la gravedad antes de salir de la escena");
        endedGame = true;
        Physics2D.gravity = new Vector2(0, init_gravity.y);
    }
}
