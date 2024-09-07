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
        public static readonly Vector2 getDependencyProgress = new Vector2(0.2f, 0.4f);
        public static readonly Vector2 collectBundleInfoProgress = new Vector2(0.4f, 0.5f);

        private static readonly Profiler ms_BuildProfiler = new Profiler(nameof(Builder));
        private static readonly Profiler ms_LoadBuildSettingProfiler = ms_BuildProfiler.CreateChild(nameof(LoadSetting));
        private static readonly Profiler ms_SwitchPlatformProfiler = ms_BuildProfiler.CreateChild(nameof(SwitchPlatform));
        private static readonly Profiler ms_CollectProfiler = ms_BuildProfiler.CreateChild(nameof(Collect));
        private static readonly Profiler ms_CollectBuildSettingFileProfiler = ms_CollectProfiler.CreateChild("CollectBuildSettingFile");
        private static readonly Profiler ms_CollectDependencyProfiler = ms_CollectProfiler.CreateChild(nameof(CollectDependency));
        private static readonly Profiler ms_CollectBundleProfiler = ms_CollectProfiler.CreateChild(nameof(CollectBundle));

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

            //�Ѽ������ļ���������ϵ
            ms_CollectDependencyProfiler.Start();
            Dictionary<string, List<string>> dependencyDic = CollectDependency(files);
            ms_CollectDependencyProfiler.Stop();

            //���������Դ����Ϣ
            Dictionary<string, EResourceType> assetDic = new Dictionary<string, EResourceType>();

            //��������÷�������ֱ������ΪDirect
            foreach (string url in files)
            {
                assetDic.Add(url, EResourceType.Direct);
            }

            //��������Դ���ΪDependency���Ѿ����ڵ�˵���˾���Direct����Դ
            foreach (string url in dependencyDic.Keys)
            {
                if (!assetDic.ContainsKey(url))
                {
                    assetDic.Add(url, EResourceType.Dependency);
                }
            }

            //���ֶα���Bundle��Ӧ����Դ����
            ms_CollectBundleProfiler.Start();
            Dictionary<string, List<string>> bundleDic = CollectBundle(buildSetting, assetDic, dependencyDic);
            ms_CollectBundleProfiler.Stop();
        }

        /// <summary>
        /// �Ѽ�ָ���ļ����ϵ�����������Ϣ
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private static Dictionary<string, List<string>> CollectDependency(ICollection<string> files)
        {
            float min = collectBundleInfoProgress.x;
            float max = collectBundleInfoProgress.y;

            Dictionary<string, List<string>> dependencyDic = new Dictionary<string, List<string>>();

            //����fileList�󣬲���Ҫ�ݹ���
            List<string> fileList = new List<string>(files);

            for (int i = 0; i < fileList.Count; i++)
            {
                string assetUrl = fileList[i];
                if (dependencyDic.ContainsKey(assetUrl))
                {
                    continue;
                }
                if (i % 10 == 0)
                {
                    //ֻ�ܴ��ģ�����
                    float progress = min + (max - min) * (float)i / (files.Count * 3);
                    EditorUtility.DisplayProgressBar($"{nameof(CollectDependency)}", "�Ѽ�������Ϣ", progress);
                }

                string[] dependencies = AssetDatabase.GetDependencies(assetUrl, false);

                List<string> dependencyList = new List<string>(dependencies.Length);

                //���˲�����Ҫ���Asset
                for (int j = 0; j < dependencies.Length; j++)
                {
                    string tempAssetUrl = dependencies[j];
                    string extension = Path.GetExtension(tempAssetUrl).ToLower();
                    if (string.IsNullOrEmpty(extension) || extension == ".cs" || extension == ".dll")
                    {
                        continue;
                    }
                    dependencyList.Add(tempAssetUrl);
                    if (!fileList.Contains(tempAssetUrl))
                    {
                        fileList.Add(tempAssetUrl);
                    }
                }

                dependencyDic.Add(assetUrl, dependencyList);
            }

            return dependencyDic;
        }

        private static Dictionary<string, List<string>> CollectBundle(BuildSetting buildSetting, Dictionary<string, EResourceType> assetDic, Dictionary<string, List<string>> dependencyDic)
        {
            float min = collectBundleInfoProgress.x;
            float max = collectBundleInfoProgress.y;

            EditorUtility.DisplayProgressBar($"{nameof(CollectBundle)}", "�Ѽ�bundle��Ϣ", min);

            Dictionary<string, List<string>> bundleDic = new Dictionary<string, List<string>>();
            //�ⲿ��Դ
            List<string> notInRuleList = new List<string>();

            int index = 0;
            foreach (KeyValuePair<string, EResourceType> pair in assetDic)
            {
                index++;
                string assetUrl = pair.Key;
                string bundleName = buildSetting.GetBundleName(assetUrl);

                //û��bundleName����ԴΪ�ⲿ��Դ
                if (bundleName == null)
                {
                    notInRuleList.Add(assetUrl);
                    continue;
                }

                List<string> list;
                if (!bundleDic.TryGetValue(bundleName, out list))
                {
                    list = new List<string>();
                    bundleDic.Add(bundleName, list);
                }

                list.Add(assetUrl);

                EditorUtility.DisplayProgressBar($"{nameof(CollectBundle)}", "�Ѽ�bundle��Ϣ", min + (max - min) * ((float)index / assetDic.Count));
            }
            if (notInRuleList.Count > 0)
            {
                string message = string.Empty;
                for (int i = 0; i < notInRuleList.Count; i++)
                {
                    message += "\n" + notInRuleList[i];
                }
                EditorUtility.ClearProgressBar();
                throw new Exception($"��Դ���ڴ������,���ߺ�׺��ƥ��!!!{message}");
            }

            foreach (List<string> list in bundleDic.Values)
            {
                list.Sort();
            }
            return bundleDic;
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
            string[] files = Directory.GetFiles(path, $"*.*", SearchOption.AllDirectories);
            List<string> result = new List<string>();

            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i].Replace('\\', '/');

                if (prefix != null && file.StartsWith(prefix, StringComparison.InvariantCulture))
                {
                    continue;
                }

                if (suffixes != null && suffixes.Length > 0)
                {
                    bool exist = false;
                    for (int j = 0; j < suffixes.Length; j++)
                    {
                        string suffix = suffixes[j];
                        if (file.EndsWith(suffix, StringComparison.InvariantCulture))
                        {
                            exist = true;
                            break;
                        }
                    }

                    if (!exist)
                    {
                        continue;
                    }
                }
                result.Add(file);
            }


            return result;
        }

        #endregion
    }

}
