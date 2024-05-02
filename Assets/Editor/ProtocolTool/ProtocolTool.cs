using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;


public class ProtocolTool
{
    //�����ļ�����·��
    private static string PROTO_INFO_PATH = Application.dataPath + "/Editor/ProtocolTool/ProtocolInfo.xml";

    private static GenerateCSharp generateCSharp = new GenerateCSharp();

    [MenuItem("ProtocolTool/����CSharp�ű�")]
    private static void GenerateCSharp()
    {
        //1.��ȡXML��ص���Ϣ
        //XmlNodeList list = GetNodes("enum");
        //������Щ��Ϣ ƴ���ַ��� ���ɶ�Ӧö�ٽű�
        generateCSharp.GenerateEnum(GetNodes("enum"));
        //���ɶ�Ӧ���ݽṹ�ű�
        generateCSharp.GenerateData(GetNodes("data"));
        //���ɶ�Ӧ��Ϣ��ű�
        generateCSharp.GenerateMsg(GetNodes("message"));
        //ˢ�±༭������ �����ֶ�ˢ��
        AssetDatabase.Refresh();
    }

    [MenuItem("ProtocolTool/����C++�ű�")]
    private static void GenerateCPlusPlus()
    {

    }

    [MenuItem("ProtocolTool/����Java�ű�")]
    private static void GenerateJava()
    {

    }
    /// <summary>
    /// ��ȡָ�����������ӽڵ��List
    /// </summary>
    /// <param name="nodeName"></param>
    /// <returns></returns>
    private static XmlNodeList GetNodes(string nodeName)
    {
        XmlDocument xml = new XmlDocument();
        xml.Load(PROTO_INFO_PATH);
        XmlNode root = xml.SelectSingleNode("message");
        return root.SelectNodes(nodeName);
    }
}
