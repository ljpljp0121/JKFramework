using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class HTTpLesson1 : MonoBehaviour
{
    void Start()
    {
        //�����Դ������
        try
        {

            //����HTTPͨѶ�����Ӷ���
            HttpWebRequest req = HttpWebRequest.Create(new Uri("http://10.163.52.189:8080/HTTP_Server/est.txt")) as HttpWebRequest;

            //������������ ����������
            req.Method = WebRequestMethods.Http.Head;
            //��ʱ
            req.Timeout = 3000;

            //��������
            HttpWebResponse res = req.GetResponse() as HttpWebResponse;

            if (res.StatusCode == HttpStatusCode.OK)
            {
                print("�ļ�����");
                print(res.ContentLength);
                print(res.ContentType);
                res.Close();
            }
            else
            {
                print("�ļ�������" + res.StatusCode);
            }
        }
        catch (WebException ex)
        {
            print("��ȡ����" + ex.Message + ex.Status);
        }

        //������Դ
        try
        {
            HttpWebRequest req = HttpWebRequest.Create(new Uri("http://10.163.52.189:8080/HTTP_Server/est.txt")) as HttpWebRequest;
            //������������ ����������
            req.Method = WebRequestMethods.Http.Get;
            req.Timeout = 3000;
            //��������
            HttpWebResponse res = req.GetResponse() as HttpWebResponse;
            //��ȡ��Ӧ������ д�뱾��·��
            if (res.StatusCode == HttpStatusCode.OK)
            {
                print(Application.persistentDataPath);
                using (FileStream fileStream = File.Create(Application.persistentDataPath + "/httpDownLoad.txt"))
                {
                    Stream downLoadStream = res.GetResponseStream();
                    byte[] bytes = new byte[2048];
                    int contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                    //����д��
                    while (contentLength != 0)
                    {
                        fileStream.Write(bytes, 0, contentLength);
                        contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                    }
                    fileStream.Close();
                    downLoadStream.Close();
                    res.Close();
                }
                print("���ؽ���");
            }
            else
            {
                print("����ʧ��" + res.StatusCode);
            }
        }
        catch (WebException ex)
        {
            print("���س���" + ex.Message + ex.Status);
        }
    }

    void Update()
    {

    }
}
