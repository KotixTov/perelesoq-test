using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoorUIView : DeviceUIView<DoorModel>
{
    [SerializeField] private Button _toggleButton;
    [SerializeField] private TMP_Text _buttonText;
    
    protected override void OnInitialize()
    {
        Model.DoorStateChanged += OnDoorStateChanged;
        Model.StateChanged += OnDoorPowerStateChanged;
        _toggleButton.onClick.AddListener(() => Model.TryToggle());
        _toggleButton.interactable = Model.IsPowered;
        _buttonText.text = Model.State == DoorState.Closed ? "Open" : "Close";
        _statusText.text = Model.State == DoorState.Closed ? "closed" : "opened";
    }

    public void OnDestroy()
    {
        Model.DoorStateChanged -= OnDoorStateChanged;
        Model.StateChanged -= OnDoorPowerStateChanged;
        _toggleButton.onClick.RemoveListener(() => Model.TryToggle());
    }

    private void OnDoorPowerStateChanged(bool power)
    {
        _toggleButton.interactable = power;
    }

    private void OnDoorStateChanged(DoorState state)
    {
        if(state == DoorState.Opening || state == DoorState.Closing)
        {
            _toggleButton.interactable = false;
            _buttonText.text = state == DoorState.Opening ? "Opening" : "Closing";
            _statusText.text = state == DoorState.Opening ? "opening" : "closing";
        }
        else
        {
            _toggleButton.interactable = Model.IsPowered;
            _buttonText.text = state == DoorState.Opened ? "Close" : "Open";
            _statusText.text = state == DoorState.Opened ? "opened" : "closed";
        }
    }
}
