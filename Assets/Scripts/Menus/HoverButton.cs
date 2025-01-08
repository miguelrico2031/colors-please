using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    public float scale = 1.2f;
    public float time = 0.5f;
    private LTDescr tweenId;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tweenId = LeanTween.scale(gameObject, Vector2.one * scale, time).setEase(LeanTweenType.easeOutElastic);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tweenId != null)
        {
            LeanTween.cancel(gameObject, tweenId.id);
            LeanTween.scale(gameObject, Vector2.one, time).setEase(LeanTweenType.easeOutCubic);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (tweenId != null)
        {
            LeanTween.cancel(gameObject, tweenId.id);
            LeanTween.scale(gameObject, Vector2.one, time).setEase(LeanTweenType.easeOutCubic);
        }
    }
}
