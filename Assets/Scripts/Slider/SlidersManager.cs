using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlidersManager : MonoBehaviour
{
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;
    public Image targetColorDisplay;
    public Button acceptButton;
    public TMP_Text feedbackText;

    private RGB255 targetColor;
    private RGB255 playerColor;
    private int attemptsLeft = 3;

    void Start()
    {
        if (redSlider == null || greenSlider == null || blueSlider == null ||
            targetColorDisplay == null || acceptButton == null || feedbackText == null)
        {
            Debug.LogError("ERROR: NO ESTA REFERENCIADO ALGO DESDE EL EDITOR");
            return;
        }

        targetColor = RGB255.Random();
        targetColorDisplay.color = targetColor.ToColor();

        redSlider.value = 0;
        greenSlider.value = 0;
        blueSlider.value = 0;

        acceptButton.onClick.AddListener(SubmitColor);
    }

    void SubmitColor()
    {
        playerColor = new RGB255(
            (byte)Mathf.Clamp(Mathf.RoundToInt(redSlider.value * 255), 0, 255),
            (byte)Mathf.Clamp(Mathf.RoundToInt(greenSlider.value * 255), 0, 255),
            (byte)Mathf.Clamp(Mathf.RoundToInt(blueSlider.value * 255), 0, 255)
        );

        if (playerColor.Equals(targetColor))
        {
            ServiceLocator.Get<IDayService>().FinishMinigame(targetColor, playerColor);
            return;
        }

        attemptsLeft--;

        if (attemptsLeft > 0)
        {
            feedbackText.text = GenerateFeedback();
        }
        else
        {
            ServiceLocator.Get<IDayService>().FinishMinigame(targetColor, playerColor);
        }
    }

    string GenerateFeedback()
    {
        string feedback = "Pistas:\n";

        feedback += CompareValues("Rojo", playerColor.R, targetColor.R);
        feedback += CompareValues("Verde", playerColor.G, targetColor.G);
        feedback += CompareValues("Azul", playerColor.B, targetColor.B);

        feedback += $"\nIntentos restantes: {attemptsLeft}";

        return feedback;
    }

    string CompareValues(string colorName, byte playerValue, byte targetValue)
    {
        if (playerValue < targetValue)
        {
            return $"{colorName}: Más alto\n";
        }
        else if (playerValue > targetValue)
        {
            return $"{colorName}: Más bajo\n";
        }
        else
        {
            return $"{colorName}: Correcto\n";
        }
    }
}
