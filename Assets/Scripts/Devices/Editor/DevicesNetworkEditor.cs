using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using Object = UnityEngine.Object;

[EditorTool("Device Connector Tool")]
public class DevicesNetworkEditor : EditorTool
{
    public Texture2D _iconTexture;

    private DevicesNetworkConfig _config;
    private DevicesNetworkConfig _lastLoadedConfig;
    
    private DeviceView _selectedSource;
    private Dictionary<string, DeviceView> _views = new Dictionary<string, DeviceView>(); // <id, view>
    private Dictionary<DeviceView, List<string>> _connections = new Dictionary<DeviceView, List<string>>(); // <source, targetIds>
    private DeviceView _hoveredDevice;

    public override GUIContent toolbarIcon => new GUIContent("Connect", _iconTexture);

    public override void OnActivated()
    {
        if(_config == null)
        {
            return;
        }
        
        Initialize();
    }

    private void Initialize()
    {
        var views = FindObjectsOfType<DeviceView>();
        _views = views.ToDictionary(view => view.Id);
        SyncSceneWithConfig();
    }

    public override void OnToolGUI(EditorWindow window)
    {
        if (!(window is SceneView)) return;

        DrawGUI();
        if (_config == null) return;

        DrawConnections();
        DrawConnectionPreview();
        HighlightTargets();
        HandleSceneInput();
    }

    private void DrawGUI()
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(50, 10, 200, 120), GUI.skin.box);
        GUILayout.Label("Devices Connector Tool", EditorStyles.boldLabel);

        GUILayout.Label("Config", EditorStyles.label);
        var newConfig  = (DevicesNetworkConfig)EditorGUILayout.ObjectField(_config, typeof(DevicesNetworkConfig), false, GUILayout.ExpandWidth(true));

        if (_config != newConfig)
        {
            _config = newConfig;
            Initialize();
        }

        GUILayout.Space(10);
        GUILayout.Label(_selectedSource ? $"Selected: {_selectedSource.name}" : "Click on device to connect", EditorStyles.miniLabel);
        GUILayout.Space(10);
        GUILayout.Label("Alt+LMB on link to remove", EditorStyles.miniLabel);
        
        GUILayout.EndArea();
        Handles.EndGUI();
    }
    private void SyncSceneWithConfig()
    {
        if (_config == null || _views == null) return;

        _connections.Clear();
        
        foreach (var configItem in _config.Devices)
        {
            if (!_views.TryGetValue(configItem.Id, out var sourceView)) continue;

            _connections.Add(sourceView, new List<string>());
            
            foreach (var outputId in configItem.OutputIds)
            {
                if (_views.TryGetValue(outputId, out var targetView))
                {
                    _connections[sourceView].Add(targetView.Id);
                }
            }
        }
    }

    private void DrawConnections()
    {
        foreach (var view in _views)
        {
            if (!_connections.TryGetValue(view.Value, out var targets))
            {
                continue;
            }
            
            foreach (var outputId in targets)
            {
                if (view.Value == null || outputId == null) continue;

                Vector3 start = view.Value.transform.position;
                Vector3 end = _views[outputId].transform.position;
                
                
                Handles.BeginGUI();
                Vector3 startGUI = HandleUtility.WorldToGUIPointWithDepth(start);
                Vector3 endGUI = HandleUtility.WorldToGUIPointWithDepth(end);
                Handles.EndGUI();
                
                Handles.color = Color.cyan;
                    
                if (startGUI.z > 0 && endGUI.z > 0)
                {
                    startGUI.z = endGUI.z = 0;
                    
                    if (IsMouseNearLine(startGUI, endGUI, 10f))
                    {
                        Handles.color = Color.yellow;
                    }
                        
                }

                Handles.DrawLine(start, end);
            }
        }
    }

    private void DrawConnectionPreview()
    {
        if (_selectedSource == null) return;

        Handles.BeginGUI();
        Handles.color = Color.yellow;
        Vector3 start = HandleUtility.WorldToGUIPoint(_selectedSource.transform.position);
        Vector3 end = Event.current.mousePosition;
        Handles.DrawLine(start, end);
        Handles.EndGUI();
    }

    private void HighlightTargets()
    {
        var e = Event.current;
        if (e.type == EventType.MouseMove)
        {
            GameObject go = HandleUtility.PickGameObject(e.mousePosition, false);
            _hoveredDevice = go ? go.GetComponentInParent<DeviceView>() : null;
        }

        foreach (var view in _views)
        {
            Color color = view.Value == _selectedSource ? Color.green :
                          view.Value == _hoveredDevice ? Color.yellow :
                          view.Value is PowerSourceView ? Color.magenta :
                          Color.cyan;
            DrawOutline(view.Value, color);
        }
    }

    private void DrawOutline(DeviceView view, Color color)
    {
        if (!view) return;

        Handles.color = color;
        float size = HandleUtility.GetHandleSize(view.transform.position) * 0.3f;
        Handles.DrawWireCube(view.transform.position, Vector3.one * size);
    }

    private void HandleSceneInput()
    {
        Event e = Event.current;
        if (e.type != EventType.MouseDown || _views == null) return;

        if (e.button == 0)
        {
            if (e.alt)
            {
                TryRemoveConnectionAtMouse();
                return;
            }

            GameObject go = HandleUtility.PickGameObject(e.mousePosition, false);
            DeviceView clicked = go ? go.GetComponentInParent<DeviceView>() : null;

            if (clicked != null)
            {
                if (_selectedSource == null)
                {
                    _selectedSource = clicked;
                }
                else if (_selectedSource != clicked)
                {
                    Undo.RecordObjects(new Object[] {_selectedSource, _config}, "Connect Devices");
                    if (TryUpdateConfigConnection(_selectedSource, clicked))
                    {
                        if (!_connections.ContainsKey(_selectedSource))
                        {
                            _connections.Add(_selectedSource, new List<string>());
                        }
                        
                        _connections[_selectedSource].Add(clicked.Id);
                    }
                    EditorUtility.SetDirty(_config);
                    _selectedSource = null;
                }
                e.Use();
            }
            else
            {
                _selectedSource = null;
            }
        }
    }

    private void TryRemoveConnectionAtMouse()
    {
        foreach (var source in _views)
        {
            if (source.Value == null || !_connections.ContainsKey(source.Value)) continue;
            
            foreach (var targetId in _connections[source.Value])
            {
                var target = _views[targetId];
                Vector3 start = HandleUtility.WorldToGUIPoint(source.Value.transform.position);
                Vector3 end = HandleUtility.WorldToGUIPoint(target.transform.position);
                if (IsMouseNearLine(start, end, 10f))
                {
                    Undo.RecordObjects(new Object[] {target, _config}, "Connect Devices");
                    _connections[source.Value].Remove(targetId);
                    TryUpdateConfigDisconnection(source.Value, target);
                    EditorUtility.SetDirty(_config);
                    Event.current.Use();
                    return;
                }
            }
        }
    }

    private bool TryUpdateConfigConnection(DeviceView from, DeviceView to)
    {
        if (string.IsNullOrEmpty(from.Id) || string.IsNullOrEmpty(to.Id))
        {
            Debug.LogError("Device id is empty");
            return false;
        }

        var deviceConfigItem = _config.Devices.Find(d => d.Id == from.Id);
        
        if (deviceConfigItem == null)
        {
            deviceConfigItem = new DeviceConfig();
            deviceConfigItem.Id = from.Id;
            deviceConfigItem.Type = from.Type;
            deviceConfigItem.OutputIds = new List<string>();
            _config.Devices.Add(deviceConfigItem);
        }
        
        if(_config.Devices.Find(d => d.Id == to.Id) ==null)
        {
            _config.Devices.Add(new DeviceConfig
            {
                Id = to.Id,
                Type = to.Type,
                OutputIds = new List<string>()
            });
        }
        
        if (!deviceConfigItem.OutputIds.Contains(to.Id))
        {
            deviceConfigItem.OutputIds.Add(to.Id);
        }
        
        return true;
    }

    private bool TryUpdateConfigDisconnection(DeviceView from, DeviceView to)
    {
        if (string.IsNullOrEmpty(from.Id) || string.IsNullOrEmpty(to.Id))
        {
            Debug.LogError("Device id is empty");
            return false;
        }
        
        var configItem = _config.Devices.Find(d => d.Id == from.Id);
        configItem?.OutputIds.Remove(to.Id);
        
        return true;
    }

    private bool IsMouseNearLine(Vector3 start, Vector3 end, float width)
    {
        Vector2 mousePos = Event.current.mousePosition;
        return HandleUtility.DistancePointLine(mousePos, start, end) < width;
    }
}
