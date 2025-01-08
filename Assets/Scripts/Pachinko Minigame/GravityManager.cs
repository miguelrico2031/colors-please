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

        Input.gyro.enabled = true;
        initial_gravity = Physics2D.gravity;
    }

    private void Update()
    {
        ModifyGravity();
    }

    private void ResetGravity()
    {
        Physics2D.gravity = initial_gravity;
    }

    private void ModifyGravity()
    {
        Vector2 inputGravity = new Vector2(Input.gyro.gravity.x, Input.gyro.gravity.y + Input.gyro.gravity.z);
        inputGravity = new Vector2(inputGravity.x, Mathf.Clamp(inputGravity.y, -1.0f, 0f)).normalized;
        Vector2 newGravity = inputGravity * Mathf.Abs(initial_gravity.y);
        Physics2D.gravity = newGravity;
    }
}
