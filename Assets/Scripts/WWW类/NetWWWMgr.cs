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
    /// 提供外部加载资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径</param>
    /// <param name="action">加载结束后的回调</param>
    public void LoadRes<T>(string path, UnityAction<T> action)where T : class
    {
        StartCoroutine(LoadResAsync<T>(path, action));
    }

    private IEnumerator LoadResAsync<T>(string path, UnityAction<T> action)where T:class
    {
        //声明www对象用于下载或加载
        WWW www = new WWW(path);

        yield return www;

        if (www.error == null)
        {
            //根据T泛型的类型 决定使用哪种类型资源
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
            Debug.LogError( "www加载资源出错" +www.error);
        }
    }
}
