using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class DevicesNetworkConfig : ScriptableObject
{
    public List<DeviceConfig> Devices = new List<DeviceConfig>();
}

[Serializable]
public class DeviceConfig
{
    public string Id;
    public DeviceType Type;
    public List<string> OutputIds = new List<string>();
    [FormerlySerializedAs("PowerUsage")] public float powerConsumption = 0;
}
