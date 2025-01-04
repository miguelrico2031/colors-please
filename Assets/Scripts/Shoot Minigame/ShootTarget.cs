
using System;
using UnityEngine;
using UnityEngine.UI;

public class ShootTarget : MonoBehaviour
{
    [SerializeField] private Transform _leftBound, _rightBound;
    private static readonly int _leftColorHash = Shader.PropertyToID("_LeftColor");
    private static readonly int _rightColorHash = Shader.PropertyToID("_RightColor");
    
    private SpriteRenderer _spriteRenderer;
    private RGB255.Coordinate _currentCoord;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void StartCoordinate(RGB255.Coordinate coord)
    {
        _currentCoord = coord;
        
        var leftGradientColor = new RGB255().ToColor();
        var rgb = new RGB255().SetCoordinate(coord, 255);
        var rightGradientColor = rgb.ToColor();
        
        _spriteRenderer.material.SetColor(_leftColorHash, leftGradientColor);
        _spriteRenderer.material.SetColor(_rightColorHash, rightGradientColor);

        _spriteRenderer.enabled = false;
        _spriteRenderer.enabled = true;
    }

    public float GetProgress(float xCoordinate)
    {
        return Mathf.InverseLerp(_leftBound.position.x, _rightBound.position.x, xCoordinate);
    }
}
