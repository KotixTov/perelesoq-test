using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DevicesNetworkService : IFixedTickService
{
    public float TotalConsumption { get; private set; }
    public float CurrentPower { get; private set; }
    public float Time { get;  private set; }

    private readonly DevicesNetworkConfig _config;
    private readonly Dictionary<string, DeviceModel> _devices = new Dictionary<string, DeviceModel>();
    private readonly List<PowerSourceModel> _powerSources = new List<PowerSourceModel>();
    private readonly List<IPowerConsumer> _powerConsumers = new List<IPowerConsumer>();

    public DevicesNetworkService(DevicesNetworkConfig config)
    {
        _config = config;
        Init();
    }
    
    public List<DeviceModel> Init()
    {
        _devices.Clear();

        foreach (var deviceConfig in _config.Devices)
        {
            var device = DeviceFactory.CreateDevice(deviceConfig);
            _devices[deviceConfig.Id] = device;

            if (device is PowerSourceModel powerSource)
            {
                _powerSources.Add(powerSource);
            }
            
            if (device is IPowerConsumer powerConsumer)
            {
                _powerConsumers.Add(powerConsumer);
            }
        }

        foreach (var deviceConfig in _config.Devices)
        {
            var from = _devices[deviceConfig.Id];

            foreach (var id in deviceConfig.OutputIds)
            {
                if (_devices.TryGetValue(id, out var to))
                {
                    from.ConnectTo(to);
                }
                else
                {
                    Debug.LogWarning($"Device {id} not found");
                }
            }
        }
        
        foreach (var powerSource in _powerSources)
        {
            powerSource.UpdatePowerState();
        }

        return new List<DeviceModel>(_devices.Values);
    }
    
    public void FixedTick(float dt)
    {
        Time += dt;

        CurrentPower = 0;
        
        foreach (var powerConsumer in _powerConsumers)
        {
            if (powerConsumer.IsPowered)
            {
                TotalConsumption += powerConsumer.PowerConsumption / 3600 * dt;
                CurrentPower += powerConsumer.PowerConsumption;
            }
        } 
    }
    
    public DeviceModel GetDevice(string id)
    {
        if (_devices.TryGetValue(id, out var device))
        {
            return device;
        }
        
        return null;
    }

    public List<DeviceModel> GetDevices()
    {
        return _devices.Values.ToList();
    }
    
    public List<T> GetDevices<T>() where T : DeviceModel => _devices.Values.OfType<T>().ToList();
}
