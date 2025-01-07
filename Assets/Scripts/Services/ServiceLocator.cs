using System.Collections.Generic;
using System;

public static class ServiceLocator
{
    public enum FailStrategies
    {
        ThrowExceptions,
        FalseOrNull,
        OverwriteOrNull,
    }

    public static FailStrategies FailStrategy { get; set; } = FailStrategies.ThrowExceptions;
    private static readonly Dictionary<string, IService> _services = new();

    public static bool Register<T>(IService service) where T : IService
    {
        string key = typeof(T).Name;
        if (!_services.TryAdd(key, service))
        {
            switch (FailStrategy)
            {
                case FailStrategies.ThrowExceptions:
                    throw new Exception($"service {key} already registered.");
                case FailStrategies.FalseOrNull:
                    return false;
                case FailStrategies.OverwriteOrNull:
                    _services[key] = service;
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        _services[key] = service;
        return true;
    }
        
    public static T Get<T>() where T : IService
    {
        string key = typeof(T).Name;
            
        if (_services.TryGetValue(key, out var service))
            return (T) service;

        return FailStrategy switch
        {
            FailStrategies.ThrowExceptions => throw new Exception($"service {key} not found."),
            FailStrategies.FalseOrNull => default,
            FailStrategies.OverwriteOrNull => default,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static void Clear()
    {
        _services.Clear();
    }
}