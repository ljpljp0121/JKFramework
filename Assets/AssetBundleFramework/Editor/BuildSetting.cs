using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

namespace AssetBundleFramework.Editor
{
    public class BuildSetting : ISupportInitialize
    {
        [DisplayName("��Ŀ����")]
        [XmlAttribute("ProjectName")]
        public string projectName { get; set; }

        [DisplayName("��׺�б�")]
        [XmlElement("SuffixList")]
        public List<string> suffixList { get; set; } = new List<string>();

        [DisplayName("����ļ���Ŀ¼�ļ���")]
        [XmlAttribute("BuildRoot")]
        public string buildRoot { get; set; }

        [DisplayName("���ѡ��")]
        [XmlElement("BuildItem")]
        public List<BuildItem> items { get; set; } = new List<BuildItem>();

        [XmlIgnore]
        public Dictionary<string, BuildItem> itemDic = new Dictionary<string, BuildItem>();

        public void BeginInit()
        {

        }

        public void EndInit()
        {
            buildRoot = Path.GetFullPath(buildRoot).Replace("\\", "/");

            itemDic.Clear();

            for (int i = 0; i < items.Count; i++)
            {
                BuildItem buildItem = items[i];

                if (buildItem.bundleType == EBundleType.All || buildItem.bundleType == EBundleType.Directory)
                {
                    if (!Directory.Exists(buildItem.assetPath))
                    {
                        throw new Exception($"��������Դ·��: {buildItem.assetPath}");
                    }
                }

                //���ݺ�׺����
                string[] prefixes = buildItem.suffix.Split('|');
                for (int j = 0; j < prefixes.Length; j++)
                {
                    string prefix = prefixes[j].Trim();
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        buildItem.suffixes.Add(prefix);
                    }
                }

                if (itemDic.ContainsKey(buildItem.assetPath))
                {
                    throw new Exception($"�ظ�����Դ·��: {buildItem.assetPath}");
                }
                itemDic.Add(buildItem.assetPath, buildItem);
            }
        }

        /// <summary>
        /// ��ȡ�����ڴ�����õ��ļ��б�
        /// </summary>
        /// <returns></returns>
        public HashSet<string> Collect()
        {
            float min = Builder.collectRuleFileProgress.x;
            float max = Builder.collectRuleFileProgress.y;
            EditorUtility.DisplayProgressBar($"{nameof(Collect)}", "�Ѽ����������Դ", min);

            //����ÿ��������Ե�Ŀ¼,��·��A/B/C,��Ҫ����A/B
            for (int i = 0; i < items.Count; i++)
            {
                BuildItem buildItem_i = items[i];
                if (buildItem_i.resourceType != EResourceType.Direct)
                    continue;

                buildItem_i.ignorPaths.Clear();
                for (int j = 0; j < items.Count; j++)
                {
                    BuildItem buildItem_j = items[j];
                    if (i != j && buildItem_j.resourceType == EResourceType.Direct)
                    {
                        if (buildItem_j.assetPath.StartsWith(buildItem_i.assetPath, StringComparison.InvariantCulture))
                        {
                            buildItem_i.ignorPaths.Add(buildItem_j.assetPath);
                        }
                    }
                }
            }

            //�洢������������������ļ�
            HashSet<string> files = new HashSet<string>();

            for (int i = 0; i < items.Count; i++)
            {
                BuildItem buildItem = items[i];

                EditorUtility.DisplayProgressBar($"{nameof(Collect)}", "�Ѽ����������Դ", min + (max - min) * ((float)i / items.Count - 1));

                if (buildItem.resourceType != EResourceType.Direct)
                    continue;

                List<string> tempFiles = Builder.GetFiles(buildItem.assetPath, null, buildItem.suffixes.ToArray());
                for (int j = 0; j < tempFiles.Count; j++)
                {
                    string file = tempFiles[j];

                    //���˱����Ե�
                    if (IsIgnore(buildItem.ignorPaths, file))
                        continue;

                    files.Add(file);
                }

                EditorUtility.DisplayProgressBar($"{nameof(Collect)}", "�Ѽ����������Դ", (float)(i + 1) / items.Count);
            }
            return files;
        }

        /// <summary>
        /// �ļ��Ƿ��ں����б���
        /// </summary>
        /// <param name="ignoreList"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool IsIgnore(List<string> ignoreList, string file)
        {
            for (int i = 0; i < ignoreList.Count; i++)
            {
                string ignorePath = ignoreList[i];
                if (string.IsNullOrEmpty(ignorePath)) continue;
                if (file.StartsWith(ignorePath, StringComparison.InvariantCulture))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// ��ȡBundleName
        /// </summary>
        /// <param name="assetUrl"></param>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        public string GetBundleName(string assetUrl, EResourceType resourceType)
        {
            //TODO
            return "";
        }
    }

}
