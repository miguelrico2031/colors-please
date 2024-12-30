using UnityEngine;
using UnityEngine.UI;

public class SlidersManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;
    public Image targetColorDisplay;
    public Button acceptButton;

    private RGB255 targetColor;
    private RGB255 playerColor;

    void Start()
    {
        if (redSlider == null || greenSlider == null || blueSlider == null ||
            targetColorDisplay == null || acceptButton == null)
        {
            Debug.LogError("ERROR: NO ESTA REFERENCIADO ALGO DESDE EL EDITOR");
            return;
        }

        targetColor = RGB255.Random();
        Debug.Log($"color meta: {targetColor.ToString()}");
        targetColorDisplay.color = targetColor.ToColor();

        redSlider.value = 0;
        greenSlider.value = 0;
        blueSlider.value = 0;

        acceptButton.onClick.AddListener(SubmitColor);
    }

    void SubmitColor() // se llama cuando se pulsa el boton de ok
    {
        playerColor = new RGB255(
            (byte)redSlider.value,
            (byte)greenSlider.value,
            (byte)blueSlider.value
        );

        Debug.Log($"color introducido: {playerColor.ToString()}");

        ServiceLocator.Get<IDayService>().FinishMinigame(targetColor, playerColor);
    }
}
