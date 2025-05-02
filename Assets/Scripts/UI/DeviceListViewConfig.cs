using System;
using UnityEngine;

[CreateAssetMenu]
public class DeviceListViewConfig : ScriptableObject
{
    public DefaultDeviceUIView DefaultDeviceUIViewTemplate;
    public DeviceUIView[] DeviceUIViewTemplates;
    public DeviceType[] ExcludedTypes;
}
