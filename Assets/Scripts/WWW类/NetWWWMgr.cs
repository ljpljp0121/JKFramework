using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class NetWWWMgr : MonoBehaviour
{
    private static NetWWWMgr instance;
    public static NetWWWMgr Instance => instance;

    private string HTTP_SERVER_PATH = "http://10.163.52.189:8080/HTTP_Server/";

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
    public void LoadRes<T>(string path, UnityAction<T> action) where T : class
    {
        StartCoroutine(LoadResAsync<T>(path, action));
    }

    private IEnumerator LoadResAsync<T>(string path, UnityAction<T> action) where T : class
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
            Debug.LogError("www加载资源出错" + www.error);
        }
    }

    /// <summary>
    /// 提供外部上传资源
    /// </summary>
    /// <param name="path">资源上传路径</param>
    /// <param name="localPath">资源所在路径</param>
    public void SendMsg<T>(string path, BaseInfo info, UnityAction<T> action) where T : BaseInfo
    {
        StartCoroutine(SendMsgAsync(info, action));
    }

    private IEnumerator SendMsgAsync<T>(BaseInfo info, UnityAction<T> action) where T : BaseInfo
    {
        WWWForm data = new WWWForm();
        //准备要发送的消息数据
        data.AddBinaryData("Info", info.WriteBytes());

        WWW www = new WWW(HTTP_SERVER_PATH, data);

        //www www =new WWW(HTTP_SERVER_PATH,info.WriteBytes());
        yield return www;
        if (www.error == null)
        {
            int index = 0;
            int infoID = BitConverter.ToInt32(www.bytes, index);
            index += 4;
            int infoLength = BitConverter.ToInt32(www.bytes, index);
            index += 4;
            //反序列化消息
            BaseInfo baseInfo = null;
            switch (infoID)
            {
                case 1001:
                    baseInfo = new PlayerInfo();
                    baseInfo.ReadBytes(www.bytes, index);
                    break;
            }
            if (baseInfo != null)
            {
                action?.Invoke(baseInfo as T);
            }
        }
        else
        {
            Debug.Log("上传失败");
        }
    }

    /// <summary>
    /// 上传文件方法
    /// </summary>
    /// <param name="fileName">上传文件名</param>
    /// <param name="localPath">本地文件路径</param>
    /// <param name="action">回调函数</param>
    public void UploadFile(string fileName, string localPath, UnityAction<UnityWebRequest.Result> action)
    {
        StartCoroutine(UploadFileAsync(fileName, localPath, action));
    }

    private IEnumerator UploadFileAsync(string fileName, string localPath, UnityAction<UnityWebRequest.Result> action)
    {
        List<IMultipartFormSection> dataList = new List<IMultipartFormSection>();
        dataList.Add(new MultipartFormFileSection(fileName, File.ReadAllBytes(localPath)));

        UnityWebRequest req = UnityWebRequest.Post(HTTP_SERVER_PATH, dataList);

        yield return req.SendWebRequest();

        action?.Invoke(req.result);

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("上传出现问题" + req.error + req.responseCode);
        }
    }

    /// <summary>
    /// 通过UnityWebRequest获取数据
    /// </summary>
    /// <typeparam name="T">byte[],Texture,AssetBundle,AudioClip,object(自定义，要保存到本地)</typeparam>
    /// <param name="path">远端上传路径</param>
    /// <param name="action">回调方法</param>
    /// <param name="localPath">本地资源路径</param>
    /// <param name="type">音频文件类型</param>
    public void UnityWebRequestLoad<T>(string path, UnityAction<T> action, string localPath = "", AudioType type = AudioType.MPEG) where T : class
    {
        StartCoroutine(UnityWebRequestLoadAsync(path, action, localPath, type));
    }

    private IEnumerator UnityWebRequestLoadAsync<T>(string path, UnityAction<T> action, string localPath = "", AudioType type = AudioType.MPEG)where T:class
    {
        UnityWebRequest req = new UnityWebRequest(path, UnityWebRequest.kHttpVerbGET);

        if (typeof(T) == typeof(byte[]))
        {
            req.downloadHandler = new DownloadHandlerBuffer();
        }
        else if (typeof(T) == typeof(Texture))
        {
            req.downloadHandler = new DownloadHandlerTexture();
        }
        else if (typeof(T) == typeof(AssetBundle))
        {
            req.downloadHandler = new DownloadHandlerAssetBundle(req.url, 0);
        }
        else if (typeof(T) == typeof(object))
        {
            req.downloadHandler = new DownloadHandlerFile(localPath);
        }
        else if (typeof(T) == typeof(AudioClip))
        {
            req = UnityWebRequestMultimedia.GetAudioClip(path,type);
        }
        else
        {
            //出现没有类型 不用继续往下执行
            Debug.Log("未知类型" + typeof(T));
            yield break;
        }

        yield return req.SendWebRequest();  

        if(req.result == UnityWebRequest.Result.Success)
        {
            if (typeof(T) == typeof(byte[]))
            {
                action?.Invoke(req.downloadHandler.data as T);
            }
            else if (typeof(T) == typeof(Texture))
            {
                //action?.Invoke((req.downloadHandler as DownloadHandlerTexture).texture as T);
                action?.Invoke((DownloadHandlerTexture.GetContent(req) as T));
            }
            else if (typeof(T) == typeof(AssetBundle))
            {
                action?.Invoke((DownloadHandlerAssetBundle.GetContent(req) as T));
            }
            else if (typeof(T) == typeof(object))
            {
                action?.Invoke(null);
            }
            else if (typeof(T) == typeof(AudioClip))
            {
                action?.Invoke((req.downloadHandler as DownloadHandlerAudioClip).audioClip as T);
            }
        }
        else
        {
            Debug.LogWarning("获取数据失败" + req.result + req.error + req.responseCode);
        }
    }
}
