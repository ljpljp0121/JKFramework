using System;
using System.IO;
using System.Net;
using UnityEngine;

public class FTPLesson20 : MonoBehaviour
{
    void Start()
    {
        try
        {
            FtpWebRequest req = FtpWebRequest.Create(new Uri("ftp://10.163.52.189/pic.png")) as FtpWebRequest;
            req.Proxy = null;
            req.Credentials = new NetworkCredential("liaojianpeng", "ljp2540297235");

            req.KeepAlive = false;
            //���ò������� �����ļ�
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            req.UseBinary = true;
            //�������ص�������
            FtpWebResponse res = req.GetResponse() as FtpWebResponse;
            Stream downLoadStream = res.GetResponseStream();

            //��ʼ����
            print(Application.persistentDataPath);
            using (FileStream fileStream = File.Create(Application.persistentDataPath + "/picture111.jpg"))
            {

                byte[] bytes = new byte[1024];
                //��ȡ����������������
                int contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                //��ͣ��ȡ�ļ��е��ֽ� ֱ������ 
                while (contentLength != 0)
                {
                    //д�뱾���ļ���
                    fileStream.Write(bytes, 0, contentLength);
                    //д���˼���д
                    contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                }
                fileStream.Close();
                downLoadStream.Close();
                print("�ϴ�����");
            }
        }
        catch (Exception e)
        {
            print("���س���" + e.Message);
        }
    }

    void Update()
    {

    }
}
