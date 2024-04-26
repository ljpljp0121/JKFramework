using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;
using UnityEngine;

public class HTTPLesson2 : MonoBehaviour
{
    void Start()
    {
        //����HttpWebRequest����
        HttpWebRequest req = HttpWebRequest.Create("http://10.163.52.189:8080/HTTP_Server/") as HttpWebRequest;
        //����
        req.Method = WebRequestMethods.Http.Post;
        req.ContentType = "multipart/form-data;boundary = LJP ";
        req.Timeout = 500000;
        //�����֤
        req.Credentials = new NetworkCredential("liaojianepng", "ljp2540297235");
        req.PreAuthenticate = true;
        //һ��Ҫ�ǵÿ���
        //����ʽƴ���ַ���תΪ�ֽ�����
        string head = "--LJP\r\n" +
            "Content-Disposition:form-data;name=\"file\";filename =\"HTTP�ϴ��ļ�.jpg\"\r\n" +
            "ContentType:application/octet-stream\r\n\r\n";
        //ͷ��ƴ���ַ���������Ϣ������
        byte[] headBytes = Encoding.UTF8.GetBytes(head);
        byte[] endBytes = Encoding.UTF8.GetBytes("\r\n--�߽��ַ���--\r\n");
        //д���ϴ���
        //�����ϴ�����
        //��д��ǰ����ͷ����Ϣ
        //��д���ļ�����
        //��д������ı߽���Ϣ
        //�ϴ����� ��ȡ��Ӧ
        using (FileStream localFileStream = File.OpenRead(Application.streamingAssetsPath + "/pic1.jpg"))
        {
            //Ҫд����ܳ���
            req.ContentLength = headBytes.Length + localFileStream.Length + endBytes.Length;
            //�����ϴ�����
            Stream upLoadStream = req.GetRequestStream();
            upLoadStream.Write(headBytes, 0, headBytes.Length);
            byte[] bytes = new byte[2048];
            int contentLength = localFileStream.Read(bytes,0,bytes.Length);
            while (contentLength != 0)
            {
                upLoadStream.Write(bytes,0,contentLength);
                contentLength = localFileStream.Read(bytes, 0, bytes.Length);
            }
            upLoadStream.Write(endBytes, 0, endBytes.Length);
            upLoadStream.Close();
            localFileStream.Close();
        }
        //�ϴ�����
        HttpWebResponse res = req.GetResponse() as HttpWebResponse;
        if (res.StatusCode == HttpStatusCode.OK)
        {
            print("�ϴ�ͨ�ųɹ�");
        }
        else
        {
            print("�ϴ�ʧ��" + res.StatusCode);
        }

    }

    void Update()
    {

    }
}
