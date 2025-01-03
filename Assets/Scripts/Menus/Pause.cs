using System;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] private bool _pausableScene = true;
    [SerializeField] private GameObject _pauseButton, _background;
    private OptionsMenu _optionsMenu;

    private float _timeScale = 1f;

    private void Awake()
    {
        _optionsMenu = GetComponent<OptionsMenu>();
        _optionsMenu.OnOptionsMenuHide += ResumeGame;
        _pauseButton.SetActive(_pausableScene);
    }

    private void OnDestroy()
    {
        if (_optionsMenu is not null)
            _optionsMenu.OnOptionsMenuHide -= ResumeGame;
    }

    public void PauseGame()
    {
        _timeScale = Time.timeScale;
        Time.timeScale = 0f;
        _pauseButton.SetActive(false);
        _optionsMenu.ShowOptions();
        _background.SetActive(true);
    }

    private void ResumeGame()
    {
        Time.timeScale = _timeScale;
        _pauseButton.SetActive(true);
        _background.SetActive(false);
    }
}