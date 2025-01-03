using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MicMinigameManager : MonoBehaviour
{
    [SerializeField] private RecordButton _recordButton;
    [SerializeField] private Image _targetColorImage;
    
    private Queue<RGB255.Coordinate> _coords = new (new [] { RGB255.Coordinate.RED , RGB255.Coordinate.GREEN, RGB255.Coordinate.BLUE});

    private RGB255 _targetColor;
    private void Start()
    {
        _targetColor = RGB255.Random();
        _targetColorImage.color = _targetColor.ToColor();
        _recordButton.OnRecordFinished += OnRecordFinished;
        NextCoord();
    }

    private void NextCoord()
    {
        if (_coords.TryDequeue(out var coord))
        {
            _recordButton.SetCoordinate(coord);
            return;
        }

        _recordButton.OnRecordFinished -= OnRecordFinished;
        ServiceLocator.Get<IDayService>().FinishMinigame(_targetColor, _recordButton.GuessedColor);
    }

    private void OnRecordFinished()
    {
        NextCoord();
    }
}