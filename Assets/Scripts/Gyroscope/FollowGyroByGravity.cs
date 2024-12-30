using UnityEngine;

public class FollowGyroByGravity : MonoBehaviour
{
    [Header("Tweaks")]
    [SerializeField] private Quaternion baseRotation = new Quaternion(0, 0, 1, 0);
    private Vector2 initialGravity;
    private Quaternion initialRotation;
    private void Start()
    {
        initialGravity = Physics2D.gravity;
        GyroManager.Instance.EnableGyro();
        initialRotation = GyroManager.Instance.GetGyroRotation();
        Debug.Log(initialRotation);
    }

    private void Update()
    {
        Calc();
    }

    private void Calc()
    {
        Quaternion currentRotation = GyroManager.Instance.GetGyroRotation();
        float rotationZ = GyroManager.Instance.GetGyroRotation().eulerAngles.z
            - initialRotation.eulerAngles.z;
        Vector2 rotatedGravity = Quaternion.Euler(0, 0, -rotationZ) * initialGravity;
        Physics2D.gravity = rotatedGravity;
    }
}
