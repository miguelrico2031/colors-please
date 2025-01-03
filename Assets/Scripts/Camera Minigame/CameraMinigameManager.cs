using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraMinigameManager : MonoBehaviour
{
    [SerializeField] private Image _targetColorImage;
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private TextMeshProUGUI _countdownText;
    [SerializeField] private int _countdownTime;
    [SerializeField] private float _countdownAnimationSize;
    [SerializeField] private float _countdownAnimationTime;
    [SerializeField] private LeanTweenType _countdownAnimationType;
    
    private WebcamManager _webcamManager;

    private void Awake()
    {
        _webcamManager = GetComponent<WebcamManager>();
        _loadingScreen.SetActive(true);
    }

    private void Start()
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        yield return new WaitUntil(() => _webcamManager.IsReady);
        
        _loadingScreen.SetActive(false);
        
        //generar un color aleatorio
        RGB255 targetColor = RGB255.Random();
        
        _targetColorImage.color = targetColor.ToColor();

        for (int i = _countdownTime; i >= 0; i--)
        {
            _countdownText.text = $"{i}";
            _countdownText.rectTransform.localScale = Vector3.one * _countdownAnimationSize;
            LeanTween.scale(_countdownText.gameObject, Vector3.one, _countdownAnimationTime)
                .setEase(_countdownAnimationType);
            yield return new WaitForSeconds(1f);
        }

        RGB255 guessedColor = FindAnyObjectByType<CameraEyedropper>().SelectedColor;

        ServiceLocator.Get<IDayService>().FinishMinigame(targetColor, guessedColor);
    }
}
