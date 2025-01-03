using UnityEngine;


public interface IMusicService
{
    void SetPhase(int phase);
    void SetSong(string songID);
    void PlaySound(string soundName);
    void PlaySoundPitch(string soundName);
    void PlaySoundPitch(string soundName, float pitchVariation);
    void PlayBackgroundSound(string soundName);
    void MuteSong();
    void ResumeSong();
    void MuteBackground();
    void ResumeBackground();
    void MenuChange(int newMenuIndex);
}
