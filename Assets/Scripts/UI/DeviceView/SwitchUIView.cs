using UnityEngine;
using UnityEngine.UI;

public class SwitchUIView : DeviceUIView<SwitchModel>
{
    [SerializeField] private Toggle toggle;

    protected override void OnInitialize()
    {
        toggle.isOn = Model.IsOn;
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
        _statusText .text = Model.IsOn ? "on" : "off";
    }

    private void OnToggleValueChanged(bool value)
    {
        Model.IsOn = value;
    }
    
    protected override void OnModelStateChanged(bool power)
    {
        _statusText.text = power ? "on" : "off";
    } 
}
