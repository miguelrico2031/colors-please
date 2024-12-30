using System;
using UnityEngine;

//Este script esta configurado en los project settings para ser ejecutado antes que los demas
public class Initializer : MonoBehaviour
{
    [Header("Money")]
    [SerializeField] private MoneyManager _moneyManager;
    
    [Header("Relationships")]
    [SerializeField] private RelationshipManager _relationshipManager;
    
    [Header("Characters")]
    [SerializeField] private CharacterManager _characterManager;
    
    [Header("Days")]
    [SerializeField] private DayManager _dayManager;
    
    [Header("Score")]
    [SerializeField] private ScoreManager _scoreManager;
    
    
    private static Initializer _instance;
    private void Awake()
    {
        if (_instance is not null)
        {
            Destroy(transform.root.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(transform.root.gameObject);
        LocateAndRegisterServices();
    }

    //La idea es, cada que se implemente un manager-service, se registre en esta funcion
    //Si hace falta cambiar una implementacion de un service por otra, se cambia aqui y gg
    private void LocateAndRegisterServices()
    {
        var audioManager = FindAnyObjectByType<AudioManager>();
        ServiceLocator.Register<IAudioService>(audioManager);
        
        var sceneManager = FindAnyObjectByType<SceneTransitionManager>();
        ServiceLocator.Register<ISceneTransitionService>(sceneManager);
        
        if (_moneyManager is null)
            throw new Exception("Money Manager not assigned.");
        ServiceLocator.Register<IMoneyService>(_moneyManager);
        
        if(_relationshipManager is null)
            throw new Exception("Relationship Manager not assigned.");
        ServiceLocator.Register<IRelationshipService>(_relationshipManager);
        
        if(_characterManager is null)
            throw new Exception("Character Manager not assigned.");
        ServiceLocator.Register<ICharacterService>(_characterManager);
        
        if(_dayManager is null)
            throw new Exception("Day Manager not assigned.");
        ServiceLocator.Register<IDayService>(_dayManager);
        
        if(_scoreManager is null)
            throw new Exception("Score Manager not assigned.");
        ServiceLocator.Register<IScoreService>(_scoreManager);
    }
}
