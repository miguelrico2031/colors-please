using System;
using TMPro;
using UnityEngine;

public class Help : MonoBehaviour
{
    [SerializeField] private GameObject _helpPanel;
    [SerializeField] private TextMeshProUGUI _helpText;

    private float _defaultTimeScale = 1f;
    private void Start()
    {
        _helpText.text = ServiceLocator.Get<IDayService>().CurrentMinigame.HelpText;
    }

    public void ToggleHelp()
    {
        if (!_helpPanel.activeSelf)
        {
            _helpPanel.SetActive(true);

            _defaultTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            
        }
        else
        {
            _helpPanel.SetActive(false);
            Time.timeScale = _defaultTimeScale;
        }
    }
}