using TMPro;
using UnityEngine;

public abstract class DeviceUIView : MonoBehaviour
{
    [SerializeField] protected TMP_Text _nameText;
    [SerializeField] protected TMP_Text _statusText;
    
    protected DeviceModel DefaultModel { get; set; }

    public abstract bool Initialize(DeviceModel model);
}


public abstract class DeviceUIView<T> : DeviceUIView where T : DeviceModel
{
    protected T Model { get; private set; }

    public override bool Initialize(DeviceModel model)
    {
        _nameText.text = model.Id;
        _statusText.text = "";
        model.StateChanged += OnModelStateChanged;
        
        if(model is T typedModel)
        {
            Model = typedModel;
            OnInitialize();
            return true;
        }

        Debug.LogError("Invalid model type");
        return false;
    }

    private void OnDestroy()
    {
        Model.StateChanged -= OnModelStateChanged;
    }

    protected virtual void OnModelStateChanged(bool power)
    { }
    protected virtual void OnInitialize()
    { }
}
