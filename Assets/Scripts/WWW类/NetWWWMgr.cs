using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NetWWWMgr : MonoBehaviour
{    
    private static NetWWWMgr instance;
    public static NetWWWMgr Instance => instance;
    private void Awake()
    {
        instance = this;
    }
    /// <summary>
    /// �ṩ�ⲿ������Դ
    /// </summary>
    /// <typeparam name="T">��Դ����</typeparam>
    /// <param name="path">��Դ·��</param>
    /// <param name="action">���ؽ�����Ļص�</param>
    public void LoadRes<T>(string path, UnityAction<T> action)where T : class
    {
        StartCoroutine(LoadResAsync<T>(path, action));
    }

    private IEnumerator LoadResAsync<T>(string path, UnityAction<T> action)where T:class
    {
        //����www�����������ػ����
        WWW www = new WWW(path);

        yield return www;

        if (www.error == null)
        {
            //����T���͵����� ����ʹ������������Դ
            if (typeof(T) == typeof(AssetBundle))
            {
                action?.Invoke(www.assetBundle as T);
            }
            else if (typeof(T) == typeof(Texture))
            {
                action?.Invoke(www.texture as T);
            }
            else if (typeof(T) == typeof(AudioClip))
            {
                action?.Invoke(www.GetAudioClip() as T);
            }
            else if (typeof(T) == typeof(string))
            {
                action?.Invoke(www.text as T);

            }
            else if (typeof(T) == typeof(byte[]))
            {
                action?.Invoke(www.bytes as T);
            }
        }
        else
        {
            Debug.LogError( "www������Դ����" +www.error);
        }
    }
}
