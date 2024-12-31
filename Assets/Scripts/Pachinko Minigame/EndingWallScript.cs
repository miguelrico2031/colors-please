using UnityEngine;

public class EndingWallScript : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Pachinko Ball"))
        {
            PachinkoManager.Instance.DetectRGBValue();
        }
    }
}
