using System;

public class CameraModel : DeviceModel
{
    public event Action<CameraModel> Activated;
    public event Action<CameraModel> Deactivated;

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive == value) return;
            
            _isActive = value;
            
            if (value)
            {
                Activated?.Invoke(this);
            }
            else
            {
                Deactivated?.Invoke(this);
            }
        }
    }
    
    private bool _isActive;
}
