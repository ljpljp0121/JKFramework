using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WWWTest : MonoBehaviour
{
    public RawImage image;
    void Start()
    {
        if (NetWWWMgr.Instance == null)
        {
            GameObject obj = new GameObject("WWW");
            obj.AddComponent<NetWWWMgr>();
        }

        NetWWWMgr.Instance.LoadRes<Texture>("https://copyright.bdstatic.com/vcg/creative/cc9c744cf9f7c864889c563cbdeddce6.jpg@h_1280", (result) =>
        {
            image.texture = result;
        });
        NetWWWMgr.Instance.LoadRes<byte[]>("https://copyright.bdstatic.com/vcg/creative/cc9c744cf9f7c864889c563cbdeddce6.jpg@h_1280", (result) =>
        {
            File.WriteAllBytes(Application.persistentDataPath + "/wwwͼƬ.jpg", result);
        });
        NetWWWMgr.Instance.SendMessage("http://10.163.52.189:8080/HTTP_Server/",
            Application.streamingAssetsPath + "pic1.jpg");
    }

    void Update()
    {

    }

    IEnumerator UpLoad()
    {
        UnityWebRequest req = new UnityWebRequest("http://10.163.52.189:8080/HTTP_Server/", UnityWebRequest.kHttpVerbPOST);

        byte[] bytes = Encoding.UTF8.GetBytes("123123123");
        req.uploadHandler = new UploadHandlerRaw(bytes);
        //req.uploadHandler.contentType = "类型/细分类型";
        yield return req.SendWebRequest();

        print(req.result);
    }
}

public class DownLoadHandleMsg : DownloadHandlerScript
{
    public BaseInfo msg;
    private byte[] cacheBytes;
    private int index = 0;
    public DownLoadHandleMsg() : base()
    {

    }

    public T GetMsg<T>() where T : BaseInfo
    {
        return msg as T;
    }

    protected override byte[] GetData()
    {
        return cacheBytes;
    }

    protected override void CompleteContent()
    {
        //默认服务器下发的是继承baseinfo的消息 所以在完成时解析它
        index = 0;
        int infoID = BitConverter.ToInt32(cacheBytes, index);
        index += 4;
        int infoLength = BitConverter.ToInt32(cacheBytes, index);
        index += 4;
        switch(infoID)
        {
            case 1001:
                msg = new PlayerInfo();
                msg.ReadBytes(cacheBytes, index);
                break;
        }
        if (msg == null)
        {
            Debug.Log("对应ID" + infoID + "没有处理");
        }
        else
        {
            Debug.Log("消息处理完毕");
        }
    }

    protected override void ReceiveContentLengthHeader(ulong contentLength)
    {
        cacheBytes = new byte[contentLength];
    }

    protected override bool ReceiveData(byte[] data, int dataLength)
    {
        data.CopyTo(cacheBytes, index);
        index += dataLength;
        return true;
    }
}
