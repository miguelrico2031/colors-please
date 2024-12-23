using UnityEngine;


public interface IAudioService : IService
{
    public void PlaySoundEffect(AudioClip clip);
    public void PlayMusic(AudioClip clip, bool loop = true);
    public void StopSoundEffects();
    public void StopMusic();
    public void ResumeMusic();
}
