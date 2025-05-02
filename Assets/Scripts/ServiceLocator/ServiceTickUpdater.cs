using System;
using UnityEngine;

public class ServiceTickUpdater : MonoBehaviour
{
    public event Action<float> Tick;
    public event Action<float> FixedTick;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update() => Tick?.Invoke(Time.deltaTime);
    private void FixedUpdate() => FixedTick?.Invoke(Time.fixedDeltaTime);
}
