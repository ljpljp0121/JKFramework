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
    /// 保存数据到XML文件
    /// </summary>
    /// <param name="data">数据对象</param>
    /// <param name="fileName">文件名</param>
    public void SaveData(object data, string fileName)
    {
        //得到存储路径
        string path = Application.persistentDataPath + "/" + fileName + ".xml";
        //存储文件
        using (StreamWriter writer = new StreamWriter(path))
        {
            //序列化
            XmlSerializer s = new XmlSerializer(data.GetType());
            s.Serialize(writer, data);
        }
    }
    /// <summary>
    /// 从XML文件中读取内容
    /// </summary>
    /// <param name="type">对象类型</param>
    /// <param name="fileName">文件名</param>
    /// <returns></returns>
    public object LoadData(Type type, string fileName)
    {
        //判断文件是否存在
        string path = Application.persistentDataPath + "/" + fileName + ".xml";
        if (!File.Exists(path))
        {
            path = Application.streamingAssetsPath + "/" + fileName + ".xml";
            if (!File.Exists(path))
            {
                //都不存在 new一个对象返回给外部 都是默认值
                return Activator.CreateInstance(type);
            }
        }
        //存在就读取
        using (StreamReader reader = new StreamReader(path))
        {
            XmlSerializer s = new XmlSerializer(type);
            return s.Deserialize(reader);
        }
    }
}
