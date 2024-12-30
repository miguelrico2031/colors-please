using UnityEngine;

public class GravityManager : MonoBehaviour
{
    private Vector2 initial_gravity;
    private void Awake()
    {
        initial_gravity = Physics2D.gravity;
        Input.gyro.enabled = true;
    }

    private void Update()
    {
        ModifyGravity();
    }

    private void ModifyGravity()
    {
        Vector2 inputGravity = new Vector2(Input.gyro.gravity.x, Input.gyro.gravity.y + Input.gyro.gravity.z);
        inputGravity = new Vector2(inputGravity.x, Mathf.Clamp(inputGravity.y, -1.0f, 0f)).normalized;
        Vector2 newGravity = inputGravity * Mathf.Abs(initial_gravity.y);
        Physics2D.gravity = newGravity;
    }

    private void ResetGravity()
    {
        Physics2D.gravity = initial_gravity;
    }
}
