using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    private EdgeCollider2D _edgeCollider;
    private Camera _mainCamera;

    private void Start()
    {
        _edgeCollider = GetComponent<EdgeCollider2D>();
        _mainCamera = Camera.main;

        AdjustCollider();
    }

    private void AdjustCollider()
    {
        Vector2[] points = new Vector2[5];
        float camHeight = _mainCamera.orthographicSize * 2;
        float camWidth = camHeight * _mainCamera.aspect;

        points[0] = new Vector2(-camWidth / 2, -camHeight / 2); // Bottom-left
        points[1] = new Vector2(-camWidth / 2, camHeight / 2);  // Top-left
        points[2] = new Vector2(camWidth / 2, camHeight / 2);   // Top-right
        points[3] = new Vector2(camWidth / 2, -camHeight / 2);  // Bottom-right
        points[4] = points[0]; // Closing the loop

        _edgeCollider.points = points;
    }
}