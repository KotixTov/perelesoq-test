using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeviceListUIBuilder : MonoBehaviour
{
    [SerializeField] private DeviceListViewConfig _deviceListViewConfig;
    [SerializeField] private Transform _container;
    
    public List<DeviceUIView> Build()
    {
        var views = new List<DeviceUIView>();
        var models = ServiceLocator.Get<DevicesNetworkService>().GetDevices();
        foreach (var model in models)
        {
            if (TryInstantiateView(model, out var view))
            {
                views.Add(view);
            }
        }

        return views;
    }

    private bool TryInstantiateView(DeviceModel model, out DeviceUIView view)
    {
        view = null;
        
        var deviceType = DeviceTypesKeeper.ModelTypes.FirstOrDefault(t => t.Value == model.GetType()).Key;

        foreach (var excludedType in _deviceListViewConfig.ExcludedTypes)
        {
            if (deviceType == excludedType)
            {
                return false;
            }
        }
        
        var deviceViewType = DeviceTypesKeeper.UIViewTypes.FirstOrDefault(t => t.Value == deviceType).Key;

        DeviceUIView viewTemplate = null;

        if (deviceViewType != null)
        {
            foreach (var template in _deviceListViewConfig.DeviceUIViewTemplates)
            {
                if (template.GetType() != deviceViewType)
                {
                    continue;
                }
                
                viewTemplate = template;
                break;
            }

            if (viewTemplate == null)
            {
                return false;
            }
        }
        else
        {
            viewTemplate = _deviceListViewConfig.DefaultDeviceUIViewTemplate;
        }

        view = Instantiate(viewTemplate, _container);
        if(view.Initialize(model))
        {
            return true;
        }
        
        Destroy(view.gameObject);
        return false;
    }
}
