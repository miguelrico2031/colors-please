
using System;
using System.Collections;
using UnityEngine;

public class Credits : MonoBehaviour
{
    [SerializeField] private GameObject _credits;
    [SerializeField] private Transform _endPos;
    [SerializeField] private float _delay, _duration;

    private IEnumerator Start()
    {
        LeanTween.moveLocalY(_credits, _endPos.position.y, _duration).setDelay(_delay).setEase(LeanTweenType.linear);
        yield return new WaitForSeconds(_duration + _delay);
        ServiceLocator.Get<ISceneTransitionService>().TransitionToScene("Main Menu");
    }
}
