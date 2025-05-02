using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DeviceListUIBuilder))]
public class DeviceListView : MonoBehaviour
{
    private List<DeviceUIView> _views;
    private DeviceListUIBuilder _builder;

    private void Awake()
    {
        _builder = GetComponent<DeviceListUIBuilder>();
    }
    
    private void Start()
    {
        _views = _builder.Build();
    }
}
