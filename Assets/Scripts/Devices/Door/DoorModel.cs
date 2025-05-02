using System;
using System.Threading;
using System.Threading.Tasks;

public enum DoorState
{
    Closed,
    Opening,
    Closing,
    Opened
}

public class DoorModel : DeviceModel, IPowerConsumer
{
    public float PowerConsumption
    {
        get => _powerConsumption;
        set { }
    }

    public event Action<DoorState> DoorStateChanged;

    public DoorState State { get; private set; }
    
    public readonly float Duration = 5f; //TODO: move to config
    private readonly float _powerConsumptionPerAction = 50f; //TODO: move to config
    
    private float _powerConsumption;

    public bool TryToggle()
    {
        if (State == DoorState.Opening || State == DoorState.Closing && !IsPowered)
        {
            return false;
        }

        Toggle();
        
        return true;
    }

    private async Task Toggle()
    {
        State = State == DoorState.Closed ? DoorState.Opening : DoorState.Closing;
        DoorStateChanged?.Invoke(State);
        
        _powerConsumption = _powerConsumptionPerAction / Duration * 3600;
        await Task.Delay((int)(Duration * 1000));

        _powerConsumption = 0f;
        State = State == DoorState.Opening ? DoorState.Opened : DoorState.Closed;
        DoorStateChanged?.Invoke(State);
    }
}
