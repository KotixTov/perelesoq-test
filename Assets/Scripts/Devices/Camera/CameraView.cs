using UnityEngine;

public class CameraView : DeviceView<CameraModel>
{
    [field: SerializeField] public Transform Pivot { get; private set; }

    public override void OnInitialize()
    {
        ServiceLocator.Get<CameraSelectorService>().Register(_model, this);
    }
}
