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
    /// �ṩ�ⲿ������Դ
    /// </summary>
    /// <typeparam name="T">��Դ����</typeparam>
    /// <param name="path">��Դ·��</param>
    /// <param name="action">���ؽ�����Ļص�</param>
    public void LoadRes<T>(string path, UnityAction<T> action) where T : class
    {
        StartCoroutine(LoadResAsync<T>(path, action));
    }

    private IEnumerator LoadResAsync<T>(string path, UnityAction<T> action) where T : class
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
            Debug.LogError("www������Դ����" + www.error);
        }
    }

    /// <summary>
    /// �ṩ�ⲿ�ϴ���Դ
    /// </summary>
    /// <param name="path">��Դ�ϴ�·��</param>
    /// <param name="localPath">��Դ����·��</param>
    public void SendMsg<T>(string path, BaseInfo info, UnityAction<T> action) where T : BaseInfo
    {
        StartCoroutine(SendMsgAsync(info, action));
    }

    private IEnumerator SendMsgAsync<T>(BaseInfo info, UnityAction<T> action) where T : BaseInfo
    {
        WWWForm data = new WWWForm();
        //׼��Ҫ���͵���Ϣ����
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
            //�����л���Ϣ
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
            Debug.Log("�ϴ�ʧ��");
        }
    }

    /// <summary>
    /// �ϴ��ļ�����
    /// </summary>
    /// <param name="fileName">�ϴ��ļ���</param>
    /// <param name="localPath">�����ļ�·��</param>
    /// <param name="action">�ص�����</param>
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
            Debug.LogWarning("�ϴ���������" + req.error + req.responseCode);
        }
    }

    /// <summary>
    /// ͨ��UnityWebRequest��ȡ����
    /// </summary>
    /// <typeparam name="T">byte[],Texture,AssetBundle,AudioClip,object(�Զ��壬Ҫ���浽����)</typeparam>
    /// <param name="path">Զ���ϴ�·��</param>
    /// <param name="action">�ص�����</param>
    /// <param name="localPath">������Դ·��</param>
    /// <param name="type">��Ƶ�ļ�����</param>
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
            //����û������ ���ü�������ִ��
            Debug.Log("δ֪����" + typeof(T));
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
            Debug.LogWarning("��ȡ����ʧ��" + req.result + req.error + req.responseCode);
        }
    }
}
