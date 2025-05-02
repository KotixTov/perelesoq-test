using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class DeviceView : MonoBehaviour
{
    [field: SerializeField]
    public string Id { get; private set; }

#if UNITY_EDITOR
    public DeviceType Type => DeviceTypesKeeper.ViewTypes[GetType()];
#endif

    private void Start()
    {
        var model = ServiceLocator.Get<DevicesNetworkService>().GetDevice(Id);
        if (model != null)
        {
            Initialize(model);
        }
        else
        {
            Debug.LogError($"Device {Id} not found");
        }
    }

    public abstract void Initialize(DeviceModel model);
    
    public virtual void OnInitialize(){}

    protected virtual void OnPowerStateChanged(bool obj)
    {}
}

public class DeviceView<T> : DeviceView where T : DeviceModel
{
    protected T _model;
    public override void Initialize(DeviceModel model)
    {
        if (model is not T typedModel)
        {
            Debug.LogError("Invalid model type");
            return;
        }
        
        _model = typedModel;
        _model.StateChanged += OnPowerStateChanged;
        
        OnInitialize();
        
        OnPowerStateChanged(_model.IsPowered);
    }
}
