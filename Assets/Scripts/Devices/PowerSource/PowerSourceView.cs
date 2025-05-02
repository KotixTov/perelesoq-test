using System;
using TMPro;
using UnityEngine;

public class PowerSourceView : DeviceView<PowerSourceModel>
{
    [SerializeField] private TMP_Text _infoText;

    private DevicesNetworkService _networkService;
    
    public override void OnInitialize()
    {
        _networkService = ServiceLocator.Get<DevicesNetworkService>();
        _infoText.text = $"TIME: 0h 0m 0s\ntotal: {_networkService.TotalConsumption:f2}W·H\nCURRENT: {_networkService.CurrentPower:f2}W";
    }

    private void Update()
    {
        TimeSpan time = TimeSpan.FromSeconds(_networkService.Time);
        _infoText.text = $"TIME: {time.Hours}h {time.Minutes}m {time.Seconds}s\ntotal: {_networkService.TotalConsumption:f2}W·H\nCURRENT: {_networkService.CurrentPower:f2}W";
    }
}
