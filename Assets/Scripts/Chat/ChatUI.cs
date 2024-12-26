using TMPro;
using UnityEngine;

public class ChatUI : MonoBehaviour
{
    //para actualizar el texto de un mensaje de chat se deben cambiar ambos a la vez, para que se resizee bien
    [SerializeField] private TextMeshProUGUI _chatDisplayedText;
    [SerializeField] private TextMeshProUGUI _chatTextToResize;
    [SerializeField] private float _initialMoveTime;

    public void Initialize(string message, Vector3 initialPosition)
    {
        _chatTextToResize.text = message;
        _chatDisplayedText.text = message;
        LeanTween.moveLocal(gameObject, initialPosition, _initialMoveTime)
            .setEase(LeanTweenType.easeOutElastic);
        
        
    }
}
