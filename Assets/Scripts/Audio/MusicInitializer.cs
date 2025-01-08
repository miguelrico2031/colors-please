using UnityEngine;

public class MusicInitializer : MonoBehaviour
{
    [SerializeField] private float delay = 0.1f;

    void Start()
    {
        StartCoroutine(InitializeMusicAfterDelay());
    }

    private System.Collections.IEnumerator InitializeMusicAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        ServiceLocator.Get<IMusicService>().SetSong("clips");
        ServiceLocator.Get<IMusicService>().SetPhase(2);
    }
}
