using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdjustColorToBackground : MonoBehaviour
{
    public GameObject targetObject; // Objeto que contiene el color de fondo

    void Start()
    {
        if (targetObject == null)
        {
            Debug.LogError("en el adjust color to background tienes que ponerle referencia al objeto del fondo que tiene el target color");
            return;
        }

        Color colorFromTarget = GetColorFromTargetObject();

        if (colorFromTarget == default)
        {
            Debug.LogError("no se ha podido coger el target color del objeto del fondo");
            return;
        }

        RGB255 targetColor = new RGB255(colorFromTarget);
        RGB255 black = new RGB255(0, 0, 0);
        float similarityWithBlack = RGB255.GetSimilarity(targetColor, black);

        Color newColor = similarityWithBlack > 0.5f ? Color.white : Color.black;

        if (TryGetComponent<TextMeshProUGUI>(out var textMeshPro)) // texto
        {
            textMeshPro.color = newColor;
        }
        else if (TryGetComponent<Image>(out var image)) // image
        {
            image.color = newColor;
        }
        else if (TryGetComponent<SpriteRenderer>(out var spriteRenderer)) // sprite
        {
            spriteRenderer.color = newColor;
        }
        else
        {
            Debug.LogWarning("no compatible el ajuste de color");
        }

        // Debug.Log($"Similarity with black: {similarityWithBlack}");
    }

    Color GetColorFromTargetObject()
    {
        if (targetObject.TryGetComponent<Image>(out var image)) // Si es imagen
        {
            return image.color;
        }
        else if (targetObject.TryGetComponent<SpriteRenderer>(out var spriteRenderer)) // Si es sprite
        {
            return spriteRenderer.color;
        }
        else
        {
            return default;
        }
    }
}
