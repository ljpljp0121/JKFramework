using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;



public class Lesson35 : MonoBehaviour
{
    void Start()
    {
        //��ȡxml�ļ���Ϣ
        XmlDocument xml = new XmlDocument();
        xml.Load(Application.dataPath + "/Scripts/Э��/Lesson.xml");
        //2 ��ȡ���ڵ�Ԫ��
        //2.1���ڵ��ȡ
        XmlNode root = xml.SelectSingleNode("message");
        //2.2��ȡ����ö�ٽṹ��ڵ�
        XmlNodeList enumList = root.SelectNodes("enum");
        foreach (XmlNode enumNode in enumList)
        {
            print("***********ö��************");
            print("ö������:" + enumNode.Attributes["name"].Value);
            print("�����ռ�:" + enumNode.Attributes["namespace"].Value);
            print("**********ö�ٳ�Ա***********");
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
        //2.3��ȡ���ݽṹ��ڵ�
        XmlNodeList dataList = root.SelectNodes("data");
        foreach (XmlNode dataNode in dataList)
        {
            print("**********���ݽṹ***********");
            print("���ݽṹ��:" + dataNode.Attributes["name"].Value);
            print("�����ռ�:" + dataNode.Attributes["namespace"].Value);
            print("*********���ݽṹ���Ա*********");
            XmlNodeList fields = dataNode.SelectNodes("field");
            foreach (XmlNode field in fields)
            {
                print(field.Attributes["type"].Value + " "
                    + field.Attributes["name"].Value + ";");
            }
        }
        //2.4��ȡ��������Ϣ�ڵ�
        XmlNodeList msgList = root.SelectNodes("message");
        foreach (XmlNode msgNode in msgList)
        {
            print("**********��Ϣ��***********");
            print("��Ϣ����:" + msgNode.Attributes["name"].Value);
            print("�����ռ�:" + msgNode.Attributes["namespace"].Value);
            print("��ϢID:" + msgNode.Attributes["id"].Value);
            print("*********��Ϣ���Ա*********");
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
