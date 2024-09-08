using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using UnityEngine;

namespace AssetBundleFramework.Editor
{
    public class BuildItem
    {
        [DisplayName("��Դ·��")]
        [XmlAttribute("AssetPath")]
        public string assetPath { get; set; }

        [DisplayName("��Դ����")]
        [XmlAttribute("ResourceType")]
        public EResourceType resourceType { get; set; } = EResourceType.Direct;

        [DisplayName("ab��������")]
        [XmlAttribute("BundleType")]
        public EBundleType bundleType { get; set; } = EBundleType.File;

        [DisplayName("��Դ��׺")]
        [XmlAttribute("Suffix")]
        public string suffix { get; set; } = ".prefab";

        [XmlIgnore]
        public List<string> ignorPaths { get; set; } = new List<string>();

        [XmlIgnore]
        public List<string> suffixes { get; set; } = new List<string>();

        //ƥ��ô�����õĸ���
        [XmlIgnore]
        public int Count { get; set; }
    }
}
