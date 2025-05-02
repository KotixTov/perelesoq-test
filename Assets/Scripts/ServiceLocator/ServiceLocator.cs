using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, IService> Services = new Dictionary<Type, IService>();
    
    private static readonly List<IFixedTickService> FixedTickServices = new List<IFixedTickService>();
    private static ServiceTickUpdater _tickUpdater;
    public static T Get<T>() where T : IService
    {
        if(Services.TryGetValue(typeof(T), out IService service))
        {
            return (T)service;
        }
        
        Debug.LogError($"Service {typeof(T).Name} not registered");
        throw new InvalidOperationException();
    }
    public static void Register<T>(T service) where T : IService
    {
        if(Services.ContainsKey(typeof(T)))
        {
            Debug.LogError($"Service {typeof(T).Name} already registered");
            return;
        }
        
        Services[typeof(T)] = service;
        
        //TODO: add ITickService

        if (service is IFixedTickService fixedTickService)
        {
            AddFixedTickService(fixedTickService);
        }
    }

    private static void AddFixedTickService(IFixedTickService fixedTickService)
    {
        if (_tickUpdater == null)
        {
            var go = new GameObject("ServiceTickUpdater");
            _tickUpdater = go.AddComponent<ServiceTickUpdater>();
            _tickUpdater.Tick += OnTick;
            _tickUpdater.FixedTick += OnFixedTick;
        }
        
        FixedTickServices.Add(fixedTickService);
    }

    private static void OnTick(float dt)
    {
        //TODO: add ITickService
    }

    private static void OnFixedTick(float dt)
    {
        foreach (var service in FixedTickServices)
        {
            service.FixedTick(dt);
        }
    }

    public static void Unregister<T>() where T : IService
    {
        if(!Services.ContainsKey(typeof(T)))
        {
            Debug.LogError($"Attempt to unregister unregistered service {typeof(T).Name}");
            return;
        }
        
        if (Services[typeof(T)] is IFixedTickService fixedTickService)
        {
            RemoveFixedTickService(fixedTickService);
        }
        
        Services.Remove(typeof(T));
    }

    private static void RemoveFixedTickService(IFixedTickService fixedTickService)
    {
        FixedTickServices.Remove(fixedTickService);
        
        if (FixedTickServices.Count == 0) //TODO: add ITickService
        {
            _tickUpdater.Tick -= OnTick;
            _tickUpdater.FixedTick -= OnFixedTick;
            GameObject.Destroy(_tickUpdater.gameObject);
            _tickUpdater = null;
        }
    }
}
