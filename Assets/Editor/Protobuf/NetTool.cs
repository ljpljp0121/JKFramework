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
    /// ���л�Protobuf����
    /// </summary>
    /// <param name="msg">���л�����Ϣ</param>
    /// <returns></returns>
    public static byte[] GetProtoBytes(IMessage msg)
    {
        //byte[] bytes = null;
        //����д��
        //using (MemoryStream ms = new MemoryStream())
        //{
        //    msg.WriteTo(ms);
        //    bytes = ms.ToArray();
        //}
        //return bytes;

        return msg.ToByteArray();
    }

    /// <summary>
    /// �����л�
    /// </summary>
    /// <typeparam name="T">��Ϣ����</typeparam>
    /// <param name="bytes">���ڷ����л����ֽ�����</param>
    /// <returns></returns>
    public static T GetProtoMsg<T>(byte[] bytes) where T : class, IMessage
    {
        //�õ���Ӧ��Ϣ����
        Type type = typeof(T);
        //ͨ������õ���Ӧ��̬��Ա���Զ���
        PropertyInfo pInfo = type.GetProperty("Parser");
        object parserObj = pInfo.GetValue(null, null);
        Type parserType = parserObj.GetType();
        MethodInfo mInfo = parserType.GetMethod("ParseFrom", new Type[] { typeof(byte[]) });
        //���ö�Ӧ���� �����л�
        object msg = mInfo.Invoke(parserObj, new object[] { bytes });
        return msg as T;
    }
}
