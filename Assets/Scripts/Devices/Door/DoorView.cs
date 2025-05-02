using System;
using UnityEngine;

public class DoorView : DeviceView<DoorModel>
{
    [SerializeField] private Transform pivot;
    [SerializeField] private float _maxAngle;

    private float _duration;
    private float _elapsedTime; 

    private bool _isMoving;
    private bool _direction;
    
    public override void OnInitialize()
    {
        _model.DoorStateChanged += OnDoorStateChanged;
        _model.StateChanged += OnDoorPowerStateChanged;
        _duration = _model.Duration;
    }

    private void OnDestroy()
    {
        _model.DoorStateChanged -= OnDoorStateChanged;
        _model.StateChanged += OnDoorPowerStateChanged;
    }

    private void Update()
    {
        if (_isMoving)
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime >= _duration)
            {
                _isMoving = false;
                _elapsedTime = 0f;
                pivot.localRotation = Quaternion.Euler(0, _direction ? _maxAngle : 0, 0);
                return;
            }

            pivot.localRotation = Quaternion.Euler(0, _direction 
                ? Mathf.Lerp(0, 90, _elapsedTime / _duration) 
                : Mathf.Lerp(90, 0, _elapsedTime / _duration), 0);
        }
    }

    private void OnDoorStateChanged(DoorState state)
    {
        if (state == DoorState.Opening || state == DoorState.Closing)
        {
            _isMoving = true;
            _direction = state == DoorState.Opening;
            _elapsedTime = 0f;
        }

        if (state == DoorState.Opened || state == DoorState.Closed)
        {
            _isMoving = false;
        }
    }

    private void OnDoorPowerStateChanged(bool power)
    {
        // TODO: make model logic first
        /*
        if (DoorState.Opening == _model.State || DoorState.Closing == _model.State)
        {
            _isMoving = power; 
        }
        */
    }
}
