using System;

public static class DeviceFactory
{
    public static DeviceModel CreateDevice(DeviceConfig deviceConfig)
    {
        var type = DeviceTypesKeeper.ModelTypes[deviceConfig.Type];
        var model = (DeviceModel)Activator.CreateInstance(type);
        model.Initialize(deviceConfig.Id);
        if (model is IPowerConsumer powerConsumer)
        {
            powerConsumer.PowerConsumption = deviceConfig.powerConsumption;
        }
        return model;
    }
}
