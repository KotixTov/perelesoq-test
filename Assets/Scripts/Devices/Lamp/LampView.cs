using System;
using UnityEngine;

[Serializable]
public class EmissionSetup
{
    public Renderer Renderer;
    [ColorUsage(false, true)] public Color Color; 
}

public class LampView : DeviceView<LampModel>
{
    [SerializeField] private EmissionSetup[] _emissionSetups;
    [SerializeField] private Light[] _lights;

    private static readonly int EmissionColorShaderID = Shader.PropertyToID("_EmissionColor");
    private void Awake()
    {
        foreach (var setup in _emissionSetups)
        {
            setup.Renderer.material.SetColor(EmissionColorShaderID, Color.black);
        }
        
        foreach (var light in _lights)
        {
            light.enabled = false;
        }
    }

    protected override void OnPowerStateChanged(bool state)
    {
        foreach (var setup in _emissionSetups)
        {
            setup.Renderer.material.SetColor(EmissionColorShaderID, state ? setup.Color : Color.black);
        }
        
        foreach (var light in _lights)
        {
            light.enabled = state;
        }
    }
}
