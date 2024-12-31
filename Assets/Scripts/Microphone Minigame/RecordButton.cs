
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecordButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RGB255 GuessedColor { get; private set; } = new RGB255();
    public event Action OnRecordFinished;
    
    [SerializeField] private MicrophoneManager _microphoneManager;
    [SerializeField] private Image _image;
    [SerializeField] private Image _iconImage;

    private RGB255.Coordinate _coord;

    private void Update()
    {
        if (!_microphoneManager.IsRecording) return;
        UpdateGuessedCoordinate(_microphoneManager.CurrentAvgVolume);
    }

    public void SetCoordinate(RGB255.Coordinate coord)
    {
        _coord = coord;
        _iconImage.color = new RGB255().SetCoordinate(coord, 255).ToColor();
        _image.color = Color.black;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _microphoneManager.StartRecording();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _microphoneManager.StopRecording();
        UpdateGuessedCoordinate(_microphoneManager.FinalAvgVolume);
        OnRecordFinished?.Invoke();
    }

    private void UpdateGuessedCoordinate(float volume)
    {
        byte amount = (byte) Mathf.RoundToInt(volume * 255);
        
        GuessedColor = GuessedColor.SetCoordinate(_coord, amount);
        _image.color = new RGB255().SetCoordinate(_coord, amount).ToColor();
    }
}
