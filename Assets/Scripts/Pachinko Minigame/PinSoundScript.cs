using UnityEngine;

public class PinSoundScript : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Pachinko Ball"))
        {
            ServiceLocator.Get<IMusicService>().PlaySoundPitch("pin1");
        }
    }
}
