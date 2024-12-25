using System;
using UnityEngine;

public class ServicesInitializer : MonoBehaviour
{
    [Header("Money")]
    [SerializeField] private MoneyManager _moneyManager;
    
    [Header("Relationships")]
    [SerializeField] private RelationshipManager _relationshipManager;
    //Este script esta configurado en los project settings para ser ejecutado antes que los demas
    private void Awake()
    {
        LocateAndRegisterServices();
    }

    //La idea es, cada que se implemente un manager-service, se registre en esta funcion
    //Si hace falta cambiar una implementacion de un service por otra, se cambia aqui y gg
    private void LocateAndRegisterServices()
    {
        var audioManager = FindAnyObjectByType<AudioManager>();
        ServiceLocator.Register<IAudioService>(audioManager);
        
        if (_moneyManager is null)
            throw new Exception("Money Manager not assigned.");
        ServiceLocator.Register<IMoneyService>(_moneyManager);
        
        if(_relationshipManager is null)
            throw new Exception("Relationship Manager not assigned.");
        ServiceLocator.Register<IRelationshipService>(_relationshipManager);
    }
}
