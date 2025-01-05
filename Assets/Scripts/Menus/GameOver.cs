
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [Serializable]
    public class GameOverData
    {
        public Character Character;
        [TextArea(5, 12)]public string Message;
    }

    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private Image _icon;
    [SerializeField] private List<GameOverData> _gameOverData;
    
    
    private void Start()
    {
        var character = ServiceLocator.Get<IDayService>().GameOverCharacter;
        var data = _gameOverData.First(d => d.Character == character);
        _gameOverText.text = data.Message;
        var sprite = ServiceLocator.Get<ICharacterService>().GetCharacterInfo(character).Picture;
        _icon.sprite = sprite;
    }

    public void ReturnToMainMenu()
    {
        ServiceLocator.Get<ISceneTransitionService>().TransitionToScene("Main Menu");
    }

    public void Retry()
    {
        MainMenuManager.LoadGame();
    }
}
