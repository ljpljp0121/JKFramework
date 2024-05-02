using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;


public class ProtocolTool
{
    //配置文件所在路径
    private static string PROTO_INFO_PATH = Application.dataPath + "/Editor/ProtocolTool/ProtocolInfo.xml";

    private static GenerateCSharp generateCSharp = new GenerateCSharp();

    [MenuItem("ProtocolTool/生成CSharp脚本")]
    private static void GenerateCSharp()
    {
        //1.读取XML相关的信息
        //XmlNodeList list = GetNodes("enum");
        //根据这些信息 拼接字符串 生成对应枚举脚本
        generateCSharp.GenerateEnum(GetNodes("enum"));
        //生成对应数据结构脚本
        generateCSharp.GenerateData(GetNodes("data"));
        //生成对应消息类脚本
        generateCSharp.GenerateMsg(GetNodes("message"));
        //刷新编辑器界面 不用手动刷新
        AssetDatabase.Refresh();
    }

    [MenuItem("ProtocolTool/生成C++脚本")]
    private static void GenerateCPlusPlus()
    {

    }

    [MenuItem("ProtocolTool/生成Java脚本")]
    private static void GenerateJava()
    {

    }
    /// <summary>
    /// 获取指定名字所有子节点的List
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
