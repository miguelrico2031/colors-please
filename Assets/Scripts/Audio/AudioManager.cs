using UnityEngine;

public class AudioManager : MonoBehaviour, IAudioService
{
    [SerializeField] private AudioSource _sfxAudioSource;
    [SerializeField] private AudioSource _musicAudioSource;
    
    public void PlaySoundEffect(AudioClip clip)
    {
        _sfxAudioSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        _musicAudioSource.Stop();
        _musicAudioSource.clip = clip;
        _musicAudioSource.loop = loop;
        _musicAudioSource.Play();
    }

    public void StopSoundEffects()
    {
        _sfxAudioSource.Stop();
    }

    public void StopMusic()
    {
        _musicAudioSource.Pause();
    }

    public void ResumeMusic()
    {
        _musicAudioSource.Play();
    }
}
