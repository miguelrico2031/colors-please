using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    //para actualizar el texto de un mensaje de chat se deben cambiar ambos a la vez, para que se resizee bien
    [SerializeField] private TextMeshProUGUI _chatDisplayedText;
    [SerializeField] private TextMeshProUGUI _chatTextToResize;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Image _pictureImage;
    [SerializeField] private Image _chatBubbleImage;
    [Header("Initial Movement")]
    [SerializeField] private float _initialMoveTime;
    [SerializeField] private LeanTweenType _initialTweenType;
    [Header("Displace Movement")]
    [SerializeField] private float _displaceTime;
    [SerializeField] private LeanTweenType _displaceTweenType;

    public void Initialize(Message message, Vector3 initialPosition)
    {
        _chatTextToResize.text = message.Text;
        _chatDisplayedText.text = message.Text;
        var characterInfo = ServiceLocator.Get<ICharacterService>().GetCharacterInfo(message.Character);
        _nameText.text = characterInfo.Name;
        _pictureImage.sprite = characterInfo.Picture;
        _chatBubbleImage.color = characterInfo.ChatColor;
        LeanTween.moveLocal(gameObject, initialPosition, _initialMoveTime)
            .setEase(_initialTweenType);
    }

    public void Displace(Vector3 displacement)
    {
        var newPosition = GetComponent<RectTransform>().localPosition + displacement;
        LeanTween.moveLocal(gameObject, newPosition, _displaceTime)
            .setEase(_displaceTweenType);
    }
    
}
