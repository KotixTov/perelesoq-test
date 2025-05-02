using System;
using TMPro;
using UnityEngine;

public class MainScreenInfoView : MonoBehaviour
{
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private TMP_Text _totalConsumptionText;
    [SerializeField] private TMP_Text _currentPowerText;
    private DevicesNetworkService _networkService;

    private void Start()
    {
        _networkService = ServiceLocator.Get<DevicesNetworkService>();
        UpdateTexts();
    }
    
    private void Update() => UpdateTexts();

    private void UpdateTexts()
    {
        TimeSpan time = TimeSpan.FromSeconds(_networkService.Time);
        _timeText.text = $"TIME: {time.Days}d {time.Hours}h {time.Minutes}m {time.Seconds}s";
        _totalConsumptionText.text = $"TOTAL: {_networkService.TotalConsumption:f2}W·H";
        _currentPowerText.text = $"CURRENT: {_networkService.CurrentPower:f2}W";
    }
}
