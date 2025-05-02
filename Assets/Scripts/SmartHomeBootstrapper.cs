using UnityEngine;

public class SmartHomeBootstrapper : MonoBehaviour
{
    [SerializeField] private DevicesNetworkConfig _config;
    [SerializeField] private Camera _camera;
    private void Awake()
    {
        ServiceLocator.Register(new DevicesNetworkService(_config));
        ServiceLocator.Register(new CameraSelectorService(_camera));
    }
}
