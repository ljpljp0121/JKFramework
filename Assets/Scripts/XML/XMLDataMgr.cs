using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class XMLDataMgr
{
    private static XMLDataMgr instance = new XMLDataMgr();

    public static XMLDataMgr Instance => instance;

    private XMLDataMgr() { }

    /// <summary>
    /// �������ݵ�XML�ļ�
    /// </summary>
    /// <param name="data">���ݶ���</param>
    /// <param name="fileName">�ļ���</param>
    public void SaveData(object data, string fileName)
    {
        //�õ��洢·��
        string path = Application.persistentDataPath + "/" + fileName + ".xml";
        //�洢�ļ�
        using (StreamWriter writer = new StreamWriter(path))
        {
            //���л�
            XmlSerializer s = new XmlSerializer(data.GetType());
            s.Serialize(writer, data);
        }
    }
    /// <summary>
    /// ��XML�ļ��ж�ȡ����
    /// </summary>
    /// <param name="type">��������</param>
    /// <param name="fileName">�ļ���</param>
    /// <returns></returns>
    public object LoadData(Type type, string fileName)
    {
        //�ж��ļ��Ƿ����
        string path = Application.persistentDataPath + "/" + fileName + ".xml";
        if (!File.Exists(path))
        {
            path = Application.streamingAssetsPath + "/" + fileName + ".xml";
            if (!File.Exists(path))
            {
                //�������� newһ�����󷵻ظ��ⲿ ����Ĭ��ֵ
                return Activator.CreateInstance(type);
            }
        }
        //���ھͶ�ȡ
        using (StreamReader reader = new StreamReader(path))
        {
            XmlSerializer s = new XmlSerializer(type);
            return s.Deserialize(reader);
        }
    }
}
