using UnityEngine;


public interface IMusicService : IService
{
    public float MusicVolume { get; set; }
    public float SoundsVolume { get; set; }
    void SetPhase(int phase);
    void SetSong(string songID);
    void PlaySound(string soundName);
    void PlaySoundPitch(string soundName);
    void PlaySoundPitch(string soundName, float pitchVariation);
    void MuteSong();
    void ResumeSong();

}
