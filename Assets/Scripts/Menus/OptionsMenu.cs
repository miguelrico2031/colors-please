
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public event Action OnOptionsMenuHide;
    
    [SerializeField] private GameObject _optionsPanel;
    [SerializeField] private GameObject _creditsPanel;
    [SerializeField] private GameObject _backButton;
    [SerializeField] private GameObject _menuCanvas;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _soundsSlider;
    [SerializeField] private TextMeshProUGUI _comparerText;

    private Comparer[] _comparers;
    private int _comparerIndex = 0;
    private void Awake()
    {
        _comparers = Enum.GetValues(typeof(Comparer)) as Comparer[];
    }

    private void Start()
    {
        //cargar los settings de memoria
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        _musicSlider.value = musicVolume;
        SetMusicVolume(musicVolume);
        float soundsVolume = PlayerPrefs.GetFloat("SoundsVolume", 1f);
        _soundsSlider.value = soundsVolume;
        SetSoundsVolume(soundsVolume);
        Comparer comparer = (Comparer) PlayerPrefs.GetInt("Comparer", (int) ComparerTool.DefaultComparer);
        RGB255.Comparer = ComparerTool.GetColorComparer(comparer);
        _comparerText.text = comparer.ToString();
        _comparerIndex = Array.IndexOf(_comparers, comparer);
    }

    public void ShowOptions()
    {
        if(_menuCanvas) _menuCanvas.SetActive(false);
        _creditsPanel.SetActive(false);
        _optionsPanel.SetActive(true);
        _backButton.SetActive(true);
    }

    public void GoBack()
    {
        if (_creditsPanel.activeSelf)
        {
            _creditsPanel.SetActive(false);
            _optionsPanel.SetActive(true);
        }
        else
        {
            _optionsPanel.SetActive(false);
            _backButton.SetActive(false);
            if(_menuCanvas) _menuCanvas.SetActive(true);
            OnOptionsMenuHide?.Invoke();
        }
    }

    public void ShowCredits()
    {
        _creditsPanel.SetActive(true);
        _optionsPanel.SetActive(false);
        ServiceLocator.Get<IMusicService>().PlaySound("aceptar2");
    }


    public void SetMusicVolume(float volume)
    {
        ServiceLocator.Get<IMusicService>().MusicVolume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSoundsVolume(float volume)
    {
        ServiceLocator.Get<IMusicService>().SoundsVolume = volume;
        PlayerPrefs.SetFloat("SoundsVolume", volume);
        PlayerPrefs.Save();
    }

    public void NextComparer(bool right)
    {
        _comparerIndex = (_comparerIndex + (right? 1 : -1)) % _comparers.Length;
        Comparer comparer = _comparers[_comparerIndex];
        _comparerText.text = comparer.ToString();
        PlayerPrefs.SetInt("Comparer", (int) comparer);
        RGB255.Comparer = ComparerTool.GetColorComparer(comparer);
        ServiceLocator.Get<IMusicService>().PlaySound("aceptar2");
    }

    public void ResetSettings()
    {
        SetMusicVolume(1f);
        _musicSlider.value = 1f;
        SetSoundsVolume(1f);
        _soundsSlider.value = 1f;
        PlayerPrefs.SetInt("Comparer", (int) ComparerTool.DefaultComparer);
        RGB255.Comparer = ComparerTool.GetColorComparer(ComparerTool.DefaultComparer);
        _comparerIndex = Array.IndexOf(_comparers, ComparerTool.DefaultComparer);
        _comparerText.text = ComparerTool.DefaultComparer.ToString();
        ServiceLocator.Get<IMusicService>().PlaySound("aceptar2");
    }

}
