using System.Collections.Generic;
using UnityEngine;

public class CameraSelectorService : IService
{
    private readonly Dictionary<CameraModel, CameraView> _cameraModelViewPairs = new Dictionary<CameraModel, CameraView>();
    
    private CameraModel _selectedCameraModel;
    
    private readonly Camera _camera;

    public CameraSelectorService(Camera camera)
    {
        _camera = camera;
        _camera.enabled = false;
    }

    public void Register(CameraModel cameraModel, CameraView cameraView)
    {
        _cameraModelViewPairs.Add(cameraModel, cameraView);
        cameraModel.Activated += OnCameraActivated;
        
        if (_camera.enabled == false)
        {
            _camera.enabled = true;
            cameraModel.IsActive = true;
        }
    }

    private void OnCameraActivated(CameraModel camera)
    {
        if (_selectedCameraModel != null)
        {
            _selectedCameraModel.IsActive = false;
        }
        _selectedCameraModel = camera;
        var view = _cameraModelViewPairs[camera];
        _camera.transform.position = view.Pivot.position;
        _camera.transform.rotation = view.Pivot.rotation;
    }
}
