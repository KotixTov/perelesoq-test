using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, IService> Services = new Dictionary<Type, IService>();
    
    private static readonly List<Type> SceneContextServiceTypes = new List<Type>(); 
    private static readonly List<IFixedTickService> FixedTickServices = new List<IFixedTickService>();
    private static ServiceTickUpdater _tickUpdater;
    
    private static int? _activeSceneId;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnBeforeSceneLoad()
    {
        if(_activeSceneId != null)
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        _activeSceneId = SceneManager.GetActiveScene().buildIndex;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private static void OnSceneUnloaded(Scene scene)
    {
        if(scene.buildIndex == _activeSceneId.Value)
        {
            for (var i = 0; i < SceneContextServiceTypes.Count; i++)
            {
                var type = SceneContextServiceTypes[i];
                Unregister(type);
                SceneContextServiceTypes.Remove(type);
                i--;
            }
        }
    }

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
        
        Services.Add(typeof(T), service);
        
        if (service.Context == ServiceContext.Scene)
        {
            SceneContextServiceTypes.Add(typeof(T));
        }
        
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

    //TODO: make it DRY
    public static void Unregister(Type serviceType)
    {
        
        if(!Services.ContainsKey(serviceType))
        {
            Debug.LogError($"Attempt to unregister unregistered service {serviceType.Name}");
            return;
        }
        
        //TODO: add ITickService
        
        if (Services[serviceType] is IFixedTickService fixedTickService)
        {
            RemoveFixedTickService(fixedTickService);
        }
        
        Services.Remove(serviceType);
    }

    public static void Unregister<T>() where T : IService
    {
        if(!Services.ContainsKey(typeof(T)))
        {
            Debug.LogError($"Attempt to unregister unregistered service {typeof(T).Name}");
            return;
        }
        
        //TODO: add ITickService
        
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
