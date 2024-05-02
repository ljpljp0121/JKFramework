using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class WebLesson2 : MonoBehaviour
{
    void Start()
    {
        //UnityWebRequest req = UnityWebRequest.Get("");
        //UnityWebRequest req = UnityWebRequestTexture.GetTexture();
        //UnityWebRequest req = UnityWebRequestAssetBundle.GetAssetBundle();
        //UnityWebRequest req = UnityWebRequest.Put();
        //UnityWebRequest req = UnityWebRequest.Post();

        //req.isDone
        //req.downloadProgress
        //req.downloadedBytes
        //UnityWebRequest req = new UnityWebRequest();
        //请求地址
        //req.url = "服务器地址";
        //请求类型
        //req.method = UnityWebRequest.kHttpVerbGET;
        //进度
        //req.downloadProgress;
        //req.uploadProgress
        //超时
        //req.timeout = 2000
        //上传下载字节数
        //req.downloadedBytes
        //req.uploadedBytes
        //重定向次数
        //req.redirectLimit = 0;



    
    }

    IEnumerator DownLoadTex()
    {
        UnityWebRequest req = new UnityWebRequest("http://10.163.52.189:8080/HTTP_Server/est.txt", UnityWebRequest.kHttpVerbGET);
        
        
        //DownloadHandlerBuffer bufferHandler = new DownloadHandlerBuffer();
        //req.downloadHandler = bufferHandler;

        req.downloadHandler = new DownloadHandlerFile(Application.persistentDataPath+"/download.txt");
        

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            //获取字节数组
            //bufferHandler.data;
        }
        else
        {
            print("获取数据失败" + req.result + req.error + req.responseCode                                          );
        }

    }


    void Update()
    {

    }
}

public class CustomDownLoadFileHandler : DownloadHandlerScript
{
    //用于保存本地存储时的路径
    private string savePath;
    private byte[] cacheBytes ;
    private int index = 0;

    public CustomDownLoadFileHandler() : base()
    {

    }

    public CustomDownLoadFileHandler(byte[] bytes) : base(bytes)
    {

    }

    public CustomDownLoadFileHandler(string path) : base()
    {
        savePath = path;
    }

    protected override byte[] GetData()
    {
        //返回字节数组
        return cacheBytes;
    }

    //从网络收到数据后 每帧会调用的方法 会自动调用的方法
    protected override bool ReceiveData(byte[] data, int dataLength)
    {
        //return base.ReceiveData(data, dataLength);
        Debug.Log("收到数据长度:" + data.Length);
        Debug.Log("收到数据长度dataLength:" + dataLength);
        data.CopyTo(cacheBytes, index);
        index += dataLength;
        return true;
    }
    //从服务器收到 Content-Length表头时 会自动调用的方法
    protected override void ReceiveContentLengthHeader(ulong contentLength)
    {
        //base.ReceiveContentLengthHeader(contentLength);
        Debug.Log("收到数据长度" + contentLength);
        //根据收到表头 决定数组容器大小
        cacheBytes = new byte[contentLength];
    }
    //消息收完了会自动调用的方法
    protected override void CompleteContent()
    {
        //base.CompleteContent();
        Debug.Log("消息收完");
        //把收到的字节数组进行自定义处理 存储到本地
        File.WriteAllBytes(savePath, cacheBytes);



    }

}