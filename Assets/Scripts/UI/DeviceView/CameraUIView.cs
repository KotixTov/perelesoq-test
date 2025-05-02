using UnityEngine;
using UnityEngine.UI;

public class CameraUIView : DeviceUIView<CameraModel>
{
    [SerializeField] private Button _selectButton;
    
    protected override void OnInitialize()
    {
        Model.Activated += OnActivated;
        Model.Deactivated += OnDeactivated;
        _selectButton.onClick.AddListener(OnSelectButton);
        _selectButton.interactable = !Model.IsActive;
    }

    private void OnActivated(CameraModel model)
    {
        _selectButton.interactable = false;
    }

    private void OnDeactivated(CameraModel model)
    {
        _selectButton.interactable = true;
    }

    private void OnSelectButton()
    {
        Model.IsActive = true;
    }
}
