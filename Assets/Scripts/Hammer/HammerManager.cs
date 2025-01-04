using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class HammerManager : MonoBehaviour
{
    public static HammerManager Instance { get; private set; }
    private const int N_COLORS = 3;
    [SerializeField] private List<Button> buttons = new List<Button>();
    [SerializeField] private List<Image> sliderFills = new List<Image>();
    [SerializeField] private Image targetColor;
    private const int WAIT_ROUND = 0;
    private const int HIT_ROUND = 1;
    int currentRound = WAIT_ROUND;

    private float waitRoundDuration = 5f;
    public float hitRoundDuration = 1;
    private int roundTimer;

    private int color = 0;

    private RGB255 answerColor;
    public RGB255 AnswerColor { get => answerColor;  set => answerColor = value;  }

    [SerializeField] TextMeshProUGUI text;
    [SerializeField] private float _countdownAnimationSize;
    [SerializeField] private float _countdownAnimationTime;
    [SerializeField] private LeanTweenType _countdownAnimationType;
    bool firstAnim = false;

    int lastIntegerTime;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
           Destroy(this);
        }       
    }

    private void Start()
    {
        Debug.Log("HOLII");

        targetColor.color = RGB255.Random().ToColor();

        roundTimer = (int)waitRoundDuration;

        StartCoroutine(HammerMinigame());
    }

    private void OnDisable()
    {
        InputSystem.DisableDevice(LinearAccelerationSensor.current);
    }

    private IEnumerator HammerMinigame()
    {
        while (true)
        {
            if (currentRound == WAIT_ROUND)
            {
                text.text = roundTimer.ToString();
                text.rectTransform.localScale = Vector3.one * _countdownAnimationSize;
                LeanTween.scale(text.gameObject, Vector3.one, _countdownAnimationTime)
                    .setEase(_countdownAnimationType);
                if (roundTimer <= 0)
                {
                    currentRound = HIT_ROUND;
                    buttons[color].onClick.Invoke();
                    roundTimer = (int)hitRoundDuration;
                }
            }
            if (currentRound == HIT_ROUND)
            {
                text.text = "HIT!";
                if(!firstAnim)
                {
                    text.rectTransform.localScale = Vector3.one * _countdownAnimationSize;
                    LeanTween.scale(text.gameObject, Vector3.one, _countdownAnimationTime)
                        .setEase(_countdownAnimationType);
                    firstAnim = true;
                }
                if (roundTimer <= 0)
                {
                    if(color != 2)
                    {
                        currentRound = WAIT_ROUND;
                        roundTimer = (int)waitRoundDuration;
                        color++;
                        firstAnim = false;
                    }                    
                }
            }           
            yield return new WaitForSeconds(1);
            roundTimer -= 1;
        }       
    }

    private void Update()
    {
        if (LinearAccelerationSensor.current != null)
        {
            InputSystem.EnableDevice(LinearAccelerationSensor.current);
        }
       
        /*roundTimer -= Time.deltaTime;

        if (currentRound == WAIT_ROUND)
        {
            text.text = roundTimer.ToString("F0");
            if(roundTimer <= 0)
            {
                currentRound = HIT_ROUND;
                buttons[color].onClick.Invoke();
                roundTimer = hitRoundDuration;
            }
        }
        else if(currentRound == HIT_ROUND)
        {
            text.text = "HIT!";
            if (roundTimer <= 0)
            {
                currentRound = WAIT_ROUND;
                roundTimer = waitRoundDuration;
            }
        }*/
    }

    public void EndHammerMiniGame()
    {
        ServiceLocator.Get<IDayService>().FinishMinigame(new RGB255(targetColor.color), new RGB255(
            (byte)(sliderFills[0].color.r * 255f), (byte)(sliderFills[1].color.g * 255f),
            (byte)(sliderFills[2].color.b * 255f)));
    }

    private bool IsAlmostInteger(float number, float tolerance = 0.001f)
    {
        // Redondea al entero más cercano y verifica la diferencia
        float nearestInteger = Mathf.Floor(number);
        return Mathf.Abs(number - nearestInteger) < tolerance;
    }
}
