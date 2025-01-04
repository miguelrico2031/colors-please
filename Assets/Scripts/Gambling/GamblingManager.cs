using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GamblingManager : MonoBehaviour
{
    [SerializeField] List<Transform> colorColumns = new List<Transform>();
    [SerializeField] public int nSlots;
    [SerializeField] GameObject targetColor;
    RGB255 targetRGB;
    RGB255 answerRGB = new RGB255(0, 0, 0);
    [SerializeField] Transform botTransform;
    [SerializeField] Transform topTransform;
    public float botEdge;
    public float topEdge;
    public float yDistance;
    const float NO_DISTANCE = 999;

    int nStops;
    [SerializeField] GameObject backGround;

    [SerializeField] private TextMeshProUGUI _countdownText;
    [SerializeField] private int _countdownTime;
    [SerializeField] private float _countdownAnimationSize;
    [SerializeField] private float _countdownAnimationTime;
    [SerializeField] private LeanTweenType _countdownAnimationType;

    public static GamblingManager Instance { get; private set; }

    private void Awake()
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
        int childPassed;
        bool distance = false;
        float y1 = NO_DISTANCE, y2 = NO_DISTANCE;
        RGB255 initColor = RGB255.Random();
        targetRGB = RGB255.Random();
        targetColor.GetComponent<SpriteRenderer>().color = targetRGB.ToColor();
        for (int i = 0; i < colorColumns.Count; i++)
        {
            childPassed = 0;
            foreach (Transform t in colorColumns[i])
            {
                if (!distance)
                {
                    if(y1 == NO_DISTANCE)
                    {
                        y1 = t.position.y;
                    }
                    else
                    {
                        y2 = t.position.y;
                        yDistance = y1 - y2;
                        distance = true;
                    }
                }
                RGB255 newColor;
                switch (i)
                {
                    case 0:
                        newColor = new RGB255((byte)Mathf.Clamp(initColor.R - childPassed, 0, 255), 0, 0);
                        t.GetComponent<SpriteRenderer>().color = newColor.ToColor();
                        break;
                    case 1:
                        newColor = new RGB255(0, (byte)Mathf.Clamp(initColor.G - childPassed, 0, 255), 0);
                        t.GetComponent<SpriteRenderer>().color = newColor.ToColor();
                        break;
                    case 2:
                        newColor = new RGB255(0, 0, (byte)Mathf.Clamp(initColor.B - childPassed, 0, 255));
                        t.GetComponent<SpriteRenderer>().color = newColor.ToColor();
                        break;
                }
                childPassed++;
            }
        }
        botEdge = botTransform.position.y;
        topEdge = topTransform.position.y;
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        for (int i = _countdownTime; i >= 0; i--)
        {
            _countdownText.text = $"{i}";
            _countdownText.rectTransform.localScale = Vector3.one * _countdownAnimationSize;
            LeanTween.scale(_countdownText.gameObject, Vector3.one, _countdownAnimationTime)
                .setEase(_countdownAnimationType);
            yield return new WaitForSeconds(1f);
        }

        ServiceLocator.Get<IDayService>().FinishMinigame(targetRGB, answerRGB);
    }

    public void ColumnStop(Color columnColor, SlotType slotType)
    {
        SpriteRenderer backgroundSprite = backGround.GetComponent<SpriteRenderer>();
        if(backgroundSprite.color == Color.white)
        {
            backgroundSprite.color = Color.black;
        }
        switch (slotType)
        {
            case SlotType.Red:
                answerRGB = new RGB255((byte)(columnColor.r * 255), answerRGB.G, answerRGB.B);
                backgroundSprite.color = answerRGB.ToColor();
                StopSlots(colorColumns[0]);
                break;
            case SlotType.Green:
                answerRGB = new RGB255(answerRGB.R, (byte)(columnColor.g * 255), answerRGB.B);
                backgroundSprite.color = answerRGB.ToColor();
                StopSlots(colorColumns[1]);
                break;
            case SlotType.Blue:
                answerRGB = new RGB255(answerRGB.R, answerRGB.G, (byte)(columnColor.b * 255));
                backgroundSprite.color = answerRGB.ToColor();
                StopSlots(colorColumns[2]);
                break;
        }
        nStops++;
        if(nStops == 3)
        {
            ServiceLocator.Get<IDayService>().FinishMinigame(targetRGB, answerRGB);
        }
    }

    private void StopSlots(Transform column)
    {
        foreach(Transform t in column)
        {
            t.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
}
