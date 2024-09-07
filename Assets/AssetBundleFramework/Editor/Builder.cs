using AssetBundleFramework.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AssetBundleFramework.Editor
{
    public static class Builder
    {
        public static readonly Vector2 collectRuleFileProgress = new Vector2(0, 0.2f);

        private static readonly Profiler ms_BuildProfiler = new Profiler(nameof(Builder));
        private static readonly Profiler ms_LoadBuildSettingProfiler = ms_BuildProfiler.CreateChild(nameof(LoadSetting));
        private static readonly Profiler ms_SwitchPlatformProfiler = ms_BuildProfiler.CreateChild(nameof(SwitchPlatform));
        private static readonly Profiler ms_CollectProfiler = ms_BuildProfiler.CreateChild(nameof(Collect));
        private static readonly Profiler ms_CollectBuildSettingFileProfiler = ms_CollectProfiler.CreateChild("CollectBuildSettingFile");

#if UNITY_IOS
    private const string PLATFORM = "IOS";
#elif UNITY_ANDROID
    private const string PLATFORM = "Android";
#else
        private const string PLATFORM = "Windows";
#endif

        /// <summary>
        /// �������
        /// </summary>
        public static BuildSetting buildSetting { get; private set; }

        /// <summary>
        /// ���Ŀ¼
        /// </summary>
        public static string buildPath { get; set; }

        /// <summary>
        /// �������
        /// </summary>
        public readonly static string BuildSettingPath = Path.GetFullPath("BuildSetting.xml").Replace("\\", "/");

        #region BuildMenuItem
        [MenuItem("Tools/ResBuild/Windows")]
        public static void BuildWindows()
        {
            Debug.Log("��ʼBuild   Windows");
            Build();
        }

        public static void SwitchPlatform()
        {
            string platform = PLATFORM;
            switch (platform)
            {
                case "Windows":
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
                    break;
                case "IOS":
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
                    break;
                case "Android":
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                    break;
            }
        }

        private static BuildSetting LoadSetting(string settingPath)
        {
            buildSetting = XmlUtility.Read<BuildSetting>(settingPath);
            if (buildSetting == null)
            {
                throw new Exception($"Load buildSetting failed,SettingPath:{settingPath}");
            }
            (buildSetting as ISupportInitialize)?.EndInit();

            buildPath = Path.GetFullPath(buildSetting.buildRoot).Replace("\\", "/");
            if (buildPath.Length > 0 && buildPath[buildPath.Length - 1] != '/')
            {
                buildPath += "/";
            }
            buildPath += $"{PLATFORM}/";


            return buildSetting;
        }

        private static void Build()
        {
            ms_BuildProfiler.Start();

            ms_SwitchPlatformProfiler.Start();
            SwitchPlatform();
            ms_SwitchPlatformProfiler.Stop();

            ms_LoadBuildSettingProfiler.Start();
            buildSetting = LoadSetting(BuildSettingPath);
            ms_LoadBuildSettingProfiler.Stop();

            //�Ѽ�Bundle��Ϣ
            ms_CollectProfiler.Start();
            Dictionary<string, List<string>> bundleDic = Collect();
            ms_CollectProfiler.Stop();

            ms_BuildProfiler.Stop();
            Debug.Log($"������{ms_BuildProfiler}");
        }

        /// <summary>
        /// �Ѽ����bundle����Ϣ
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, List<string>> Collect()
        {
            //��ȡ�����ڴ�����õ��ļ��б�
            ms_CollectBuildSettingFileProfiler.Start();
            HashSet<string> files = buildSetting.Collect();
            ms_CollectBuildSettingFileProfiler.Stop();

            return null;
        }

        /// <summary>
        /// ��ȡָ��·�����ļ�
        /// </summary>
        /// <param name="path"></param>
        /// <param name="prefix"></param>
        /// <param name="suffixes"></param>
        /// <returns></returns>
        public static List<string> GetFiles(string path, string prefix, params string[] suffixes)
        {
            List<string> result = new List<string>();
            return result;
        }

        #endregion
    }

}
