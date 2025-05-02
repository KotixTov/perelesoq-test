using System;

public class DeviceViewModel
{
    public event Action<bool> StateChanged;

    public bool IsPowered => _model.IsPowered;

    private readonly DeviceModel _model;
    
    public DeviceViewModel(DeviceModel model)
    {
        _model = model;
        _model.StateChanged += OnStateChanged;
    }
    
    ~DeviceViewModel()
    {
        _model.StateChanged -= OnStateChanged;
    }

    private void OnStateChanged(bool state)
    {
        StateChanged?.Invoke(state);
    }
}
