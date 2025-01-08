using UnityEngine;

public class BouncyPinScript : MonoBehaviour
{
    [SerializeField] private long _vibrationMilliseconds = 100;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Pachinko Ball"))
        {
            Vibration.Vibrate(_vibrationMilliseconds);
            ServiceLocator.Get<IMusicService>().PlaySoundPitch("pin2");
        }
    }
}
