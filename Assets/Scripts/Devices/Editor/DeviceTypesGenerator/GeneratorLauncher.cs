using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class GeneratorLauncher
{
    private const string DIR ="Scripts/Devices/Types";
    public static string MakePath(string name) => $"{DIR}/{name}.cs";

    static GeneratorLauncher()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        if (!DeviceTypesGenerator.Generate())
        {
            return;
        }

        AssetDatabase.Refresh();
        Debug.Log("Devices types generated");
    }
}