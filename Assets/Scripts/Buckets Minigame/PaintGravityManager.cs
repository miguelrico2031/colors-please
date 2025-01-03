using UnityEngine;

public class PaintGravityManager : MonoBehaviour
{
    private Vector3 init_gravity;

    private void Awake()
    {
        init_gravity = Physics.gravity;
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }
        else
        {
            Debug.LogWarning("El giroscopio no está disponible en este dispositivo.");
        }
    }

    void Update()
    {
        if (Input.gyro.enabled)
        {
            Vector2 inputGravity = new Vector2(Input.gyro.gravity.x, Input.gyro.gravity.y + Input.gyro.gravity.z);
            inputGravity = new Vector2(inputGravity.x, Mathf.Clamp(inputGravity.y, -1.0f, 0f)).normalized;
            Vector2 newGravity = inputGravity * Mathf.Abs(init_gravity.y) * 2.5f;
            Physics2D.gravity = newGravity;
        }
    }
}
