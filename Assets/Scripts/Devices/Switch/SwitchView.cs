using System;
using UnityEngine;

public class SwitchView : DeviceView<SwitchModel>
{
    [SerializeField] private float _angle = 8f;
    [SerializeField] private Transform _pivot;
    
    [SerializeField] private MeshRenderer _indicator;
    [SerializeField] [ColorUsage(false, true)] private Color _indicatorColorOn;
    [SerializeField] [ColorUsage(false, true)] private Color _indicatorColorOff;
    
    private static readonly int EmissionColorShaderID = Shader.PropertyToID("_EmissionColor");

    public override void OnInitialize()
    {
        _model.IsOnChanged += OnModelIsOnChanged;
        OnModelIsOnChanged(_model.IsOn);
    }

    private void OnDestroy()
    {
        _model.IsOnChanged -= OnModelIsOnChanged;
    }

    protected override void OnPowerStateChanged(bool isPowered)
    {
        OnModelIsOnChanged(_model.IsOn);
    }

    private void OnModelIsOnChanged(bool isOn)
    {
        _pivot.localEulerAngles = new Vector3(0, 0, isOn ? _angle : -_angle);
        
        if (_model.IsPowered)
        {
            _indicator.material.SetColor(EmissionColorShaderID, isOn ? _indicatorColorOn : _indicatorColorOff);
        }
        else
        {
            _indicator.material.SetColor(EmissionColorShaderID, Color.black);
        }
    }
}
