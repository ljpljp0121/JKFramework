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
        //�����ַ
        //req.url = "��������ַ";
        //��������
        //req.method = UnityWebRequest.kHttpVerbGET;
        //����
        //req.downloadProgress;
        //req.uploadProgress
        //��ʱ
        //req.timeout = 2000
        //�ϴ������ֽ���
        //req.downloadedBytes
        //req.uploadedBytes
        //�ض������
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
            //��ȡ�ֽ�����
            //bufferHandler.data;
        }
        else
        {
            print("��ȡ����ʧ��" + req.result + req.error + req.responseCode                                          );
        }

    }


    void Update()
    {

    }
}

public class CustomDownLoadFileHandler : DownloadHandlerScript
{
    //���ڱ��汾�ش洢ʱ��·��
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
        //�����ֽ�����
        return cacheBytes;
    }

    //�������յ����ݺ� ÿ֡����õķ��� ���Զ����õķ���
    protected override bool ReceiveData(byte[] data, int dataLength)
    {
        //return base.ReceiveData(data, dataLength);
        Debug.Log("�յ����ݳ���:" + data.Length);
        Debug.Log("�յ����ݳ���dataLength:" + dataLength);
        data.CopyTo(cacheBytes, index);
        index += dataLength;
        return true;
    }
    //�ӷ������յ� Content-Length��ͷʱ ���Զ����õķ���
    protected override void ReceiveContentLengthHeader(ulong contentLength)
    {
        //base.ReceiveContentLengthHeader(contentLength);
        Debug.Log("�յ����ݳ���" + contentLength);
        //�����յ���ͷ ��������������С
        cacheBytes = new byte[contentLength];
    }
    //��Ϣ�����˻��Զ����õķ���
    protected override void CompleteContent()
    {
        //base.CompleteContent();
        Debug.Log("��Ϣ����");
        //���յ����ֽ���������Զ��崦�� �洢������
        File.WriteAllBytes(savePath, cacheBytes);



    }

}