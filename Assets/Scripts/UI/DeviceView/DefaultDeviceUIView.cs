public class DefaultDeviceUIView : DeviceUIView
{
    public override bool Initialize(DeviceModel model)
    {
        DefaultModel = model;
        _nameText.text = model.Id;
        _statusText.text = model.IsPowered ? "on" : "off";
        model.StateChanged += OnModelStateChanged;
        return true;
    }

    private void OnDestroy()
    {
        DefaultModel.StateChanged -= OnModelStateChanged;
    }

    private void OnModelStateChanged(bool power)
    {
        _statusText.text = power ? "on" : "off";
    }
}