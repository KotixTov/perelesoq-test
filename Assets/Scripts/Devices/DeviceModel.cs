using System;
using System.Collections.Generic;
using System.Linq;

public abstract class DeviceModel
{
    public string Id { get; private set; }
    
    public event Action<bool> StateChanged;
    public bool IsPowered => _inputs.Any(i => i.IsOutputPowered);
    public virtual bool IsOutputPowered => IsPowered;

    protected readonly List<DeviceModel> _outputs = new();
    protected readonly List<DeviceModel> _inputs = new();

    public void Initialize(string id)
    {
        Id = id;
        StateChanged += OnStateChanged;
        OnInitialize();
    }

    protected virtual void OnInitialize() {}
    
    ~DeviceModel() => StateChanged -= OnStateChanged;
    
    protected virtual void OnStateChanged(bool power) {}

    public void ConnectTo(DeviceModel output)
    {
        _outputs.Add(output);
        output._inputs.Add(this);
    }

    public void Disconnect(DeviceModel output)
    { 
        _outputs.Remove(output);
        output._inputs.Remove(this);
    }

    public void UpdatePowerState()
    {
        StateChanged?.Invoke(IsPowered);

        foreach (var output in _outputs)
        {
            output.UpdatePowerState();
        }
    }
}
