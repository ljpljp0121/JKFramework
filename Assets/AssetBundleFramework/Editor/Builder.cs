using AssetBundleFramework.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Builder
{
    private static readonly Profiler ms_BuildProfiler = new Profiler(nameof(Builder));
    private static readonly Profiler ms_LoadBuildSettingProfiler = ms_BuildProfiler.CreateChild(nameof(LoadSetting));
    private static readonly Profiler ms_SwitchPlatformProfiler = ms_BuildProfiler.CreateChild(nameof(SwitchPlatform));

    #region BuildMenuItem
    [MenuItem("Tools/ResBuild/Windows")]
    public static void BuildWindows()
    {

    }

    public static void SwitchPlatform()
    {

    }

    private static void LoadSetting(string settingPath)
    {

    }

    #endregion
}
