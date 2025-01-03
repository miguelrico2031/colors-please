using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    private float maxAccelerationValue = 0;
    [SerializeField] Image fill;
    [SerializeField] SlotType slotType;
    [SerializeField] Slider slider;

    private float timer;

    private float sliderUpdateDuration = 1f;
    private float sliderUpdateRate = 0.02f;

    bool coroutineStarted = false;


    void Start()
    {
        timer = 0;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < HammerManager.Instance.hitRoundDuration)
        {
            if (LinearAccelerationSensor.current == null)
            {
                Debug.LogWarning("No se puede recoger la aceleracion lineal");
                return;
            }
            else
            {
                Debug.Log("Se puede recoger la aceleracion lineal");
            }
            float accelerationValue = LinearAccelerationSensor.current.acceleration.ReadValue().magnitude;
            accelerationValue *= 255 * 0.25f;
            if (accelerationValue <= maxAccelerationValue) return;
            if (accelerationValue > 255f) accelerationValue = 255;
            maxAccelerationValue = accelerationValue;
        }
        else
        {
            if (!coroutineStarted)
            {
                coroutineStarted = true;
                StartCoroutine(UpdateSlider());
            }
        }           
    }

    private IEnumerator UpdateSlider()
    {
        float sliderTimer = 0;
        while (sliderTimer <= sliderUpdateDuration)
        {
            float lerpValue = sliderTimer / sliderUpdateDuration;
            slider.value = Mathf.Lerp(0, maxAccelerationValue, lerpValue);
            sliderTimer += sliderUpdateRate;
            switch (slotType)
            {
                case SlotType.Red:
                    fill.color = new RGB255((byte)slider.value, 0, 0).ToColor();
                    break;
                case SlotType.Green:
                    fill.color = new RGB255(0, (byte)slider.value, 0).ToColor();
                    break;
                case SlotType.Blue:
                    fill.color = new RGB255(0, 0, (byte)slider.value).ToColor();
                    break;
            }
            yield return new WaitForSeconds(sliderUpdateRate);
        }
        if(slotType == SlotType.Blue)
        {
            HammerManager.Instance.EndHammerMiniGame();
        }
    }
}
