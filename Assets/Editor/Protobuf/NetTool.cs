using GamePlayerTest;
using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public static class NetTool
{
    /// <summary>
    /// 序列化Protobuf对象
    /// </summary>
    /// <param name="msg">序列化的消息</param>
    /// <returns></returns>
    public static byte[] GetProtoBytes(IMessage msg)
    {
        //byte[] bytes = null;
        //基础写法
        //using (MemoryStream ms = new MemoryStream())
        //{
        //    msg.WriteTo(ms);
        //    bytes = ms.ToArray();
        //}
        //return bytes;

        return msg.ToByteArray();
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    /// <param name="bytes">用于反序列化的字节数组</param>
    /// <returns></returns>
    public static T GetProtoMsg<T>(byte[] bytes) where T : class, IMessage
    {
        //得到对应消息类型
        Type type = typeof(T);
        //通过反射得到对应静态成员属性对象
        PropertyInfo pInfo = type.GetProperty("Parser");
        object parserObj = pInfo.GetValue(null, null);
        Type parserType = parserObj.GetType();
        MethodInfo mInfo = parserType.GetMethod("ParseFrom", new Type[] { typeof(byte[]) });
        //调用对应方法 反序列化
        object msg = mInfo.Invoke(parserObj, new object[] { bytes });
        return msg as T;
    }
}
