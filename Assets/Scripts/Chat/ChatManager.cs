using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    [SerializeField] private ChatUI _chatUIPrefab;
    [SerializeField] private ChatUI _answerChatUIPrefab;

    [SerializeField] private RectTransform _chatUIContainer;
    [SerializeField] private RectTransform _chatSpawnPosition;
    [SerializeField] private RectTransform _chatInitialPosition;

    [SerializeField] private Button _replyButton;
    [SerializeField] private GameObject _replyArea;
    [SerializeField] private GameObject _continueArea;
    [SerializeField] private float _areaSwapDuration;

    [SerializeField] private LeanTweenType _areaSwapAnimationType;

    [SerializeField] private Vector3 _chatDisplacement;
    [SerializeField] private float _delayBeforeFirstMessage;
    [SerializeField] private float _delayBetweenMessages;
    [SerializeField] private int _initialContainterCapacity;


    private Dialogue _dialogue;
    private readonly List<ChatUI> _displayedChats = new();
    private Queue<Message> _messages;
    private bool _canReply;
    private Message _replyMessage;


    private void Start()
    {
        _dialogue = ServiceLocator.Get<IDayService>().DialogueToDisplay;
        _messages = new Queue<Message>(_dialogue.Messages);
        _replyButton.onClick.AddListener(Reply);
        StartChat();

        ServiceLocator.Get<IMusicService>().SetPhase(1);

    }

    public void StartChat()
    {
        StartCoroutine(StartChatCoroutine());
    }

    public void Reply()
    {
        if (!_canReply) return;
        _canReply = false;
        _replyButton.interactable = false;
        StartCoroutine(ReplyCoroutine());

        //ServiceLocator.Get<IMusicService>().PlaySound("aceptar");
    }

    public void Continue()
    {
        ServiceLocator.Get<IMusicService>().SetPhase(0);
        ServiceLocator.Get<IMusicService>().PlaySound("aceptar2");

        var dayService = ServiceLocator.Get<IDayService>();
        if (dayService.IsEndOfDayDialogue)
            dayService.GoToBuckets();
        else
            dayService.GoToNextMinigame();
    }

    private IEnumerator StartChatCoroutine()
    {
        _canReply = false;
        _replyButton.interactable = false;
        yield return new WaitForSeconds(_delayBeforeFirstMessage);
        yield return DisplayMessagesUntilReplyOrEnd();
    }

    private IEnumerator DisplayMessagesUntilReplyOrEnd()
    {
        while (_messages.Count > 0)
        {
            var message = _messages.Dequeue();

            if (message.Character is Character.Yourself)
            {
                _canReply = true;
                _replyButton.interactable = true;
                _replyMessage = message;
                yield break;
            }

            DisplaceChats();
            DisplayMessage(message);
            yield return new WaitForSeconds(_delayBetweenMessages);
        }

        //se ha terminado el dialogo
        var displacement = new Vector3(
            Mathf.Abs(_replyArea.transform.localPosition.x - _continueArea.transform.localPosition.x),
            0f, 0f);

        LeanTween.moveLocal(_continueArea, _replyArea.transform.localPosition, _areaSwapDuration)
            .setEase(_areaSwapAnimationType);

        LeanTween.moveLocal(_replyArea, _replyArea.transform.localPosition + displacement, _areaSwapDuration)
            .setEase(_areaSwapAnimationType);
    }

    private IEnumerator ReplyCoroutine()
    {
        DisplaceChats();
        DisplayMessage(_replyMessage, isReply: true);
        yield return new WaitForSeconds(_delayBetweenMessages);
        yield return DisplayMessagesUntilReplyOrEnd();
    }

    private void DisplayMessage(Message message, bool isReply = false)
    {
        var chatUI = Instantiate(isReply ? _answerChatUIPrefab : _chatUIPrefab, _chatSpawnPosition.position,
            _chatSpawnPosition.rotation, _chatUIContainer);
        chatUI.Initialize(message, _chatInitialPosition.localPosition);
        _displayedChats.Add(chatUI);
        ServiceLocator.Get<IMusicService>().PlaySoundPitch("exclamacion", 0.2f);
    }

    private void DisplaceChats()
    {
        foreach (var chatUI in _displayedChats)
        {
            chatUI.Displace(_chatDisplacement);
        }

        if (_displayedChats.Count > _initialContainterCapacity)
        {
            int notInCapacity = _displayedChats.Count - _initialContainterCapacity;
            var newSize = _chatInitialPosition.sizeDelta + notInCapacity * (Vector2)_chatDisplacement;
            _chatUIContainer.sizeDelta = new Vector2(_chatUIContainer.sizeDelta.x, newSize.y);
        }
    }
}