using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;



public class Lesson35 : MonoBehaviour
{
    void Start()
    {
        //读取xml文件信息
        XmlDocument xml = new XmlDocument();
        xml.Load(Application.dataPath + "/Scripts/协议/Lesson.xml");
        //2 读取各节点元素
        //2.1根节点读取
        XmlNode root = xml.SelectSingleNode("message");
        //2.2读取所有枚举结构类节点
        XmlNodeList enumList = root.SelectNodes("enum");
        foreach (XmlNode enumNode in enumList)
        {
            print("***********枚举************");
            print("枚举名字:" + enumNode.Attributes["name"].Value);
            print("命名空间:" + enumNode.Attributes["namespace"].Value);
            print("**********枚举成员***********");
            XmlNodeList fields = enumNode.SelectNodes("field");
            foreach (XmlNode fieldsNode in fields)
            {
                string str = fieldsNode.Attributes["name"].Value;
                if (fieldsNode.InnerText != "")
                {
                    str += " = " + fieldsNode.InnerText;
                    str += ",";
                    print(str);
                }
            }
        }
        //2.3读取数据结构类节点
        XmlNodeList dataList = root.SelectNodes("data");
        foreach (XmlNode dataNode in dataList)
        {
            print("**********数据结构***********");
            print("数据结构名:" + dataNode.Attributes["name"].Value);
            print("命名空间:" + dataNode.Attributes["namespace"].Value);
            print("*********数据结构类成员*********");
            XmlNodeList fields = dataNode.SelectNodes("field");
            foreach (XmlNode field in fields)
            {
                print(field.Attributes["type"].Value + " "
                    + field.Attributes["name"].Value + ";");
            }
        }
        //2.4读取出所有消息节点
        XmlNodeList msgList = root.SelectNodes("message");
        foreach (XmlNode msgNode in msgList)
        {
            print("**********消息类***********");
            print("消息类名:" + msgNode.Attributes["name"].Value);
            print("命名空间:" + msgNode.Attributes["namespace"].Value);
            print("消息ID:" + msgNode.Attributes["id"].Value);
            print("*********消息类成员*********");
            XmlNodeList fields = msgNode.SelectNodes("field");
            foreach (XmlNode field in fields)
            {
                print(field.Attributes["type"].Value + " "
                    + field.Attributes["name"].Value + ";");
            }
        }
    }

    void Update()
    {

    }
}
