using UnityEngine;

public class SwitchModel : DeviceModel
{
    public event System.Action<bool> IsOnChanged;
    public bool IsOn
    {
        get => _isOn;
        set
        {
            if(IsOn == value)
            {
                return;
            }

            _isOn = value;
            IsOnChanged?.Invoke(_isOn);
            
            foreach (var output in _outputs)
            {
                output.UpdatePowerState();
            }
        }
    }

    private bool _isOn;

    public override bool IsOutputPowered => IsPowered && IsOn;
}
