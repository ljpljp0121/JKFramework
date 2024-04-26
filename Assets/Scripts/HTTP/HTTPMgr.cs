using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class HTTPMgr
{
    private static HTTPMgr instance = new HTTPMgr();
    public static HTTPMgr Instance => instance;

    //Զ��HTTP�������ĵ�ַ
    private string HTTP_PATH = "http://10.163.52.189:8080/HTTP_Server/";

    private string USER_NAME = "liaojianpeng";
    private string PASS_WORD = "ljp2540292735";

    /// <summary>
    /// ��Http�����������ļ�
    /// </summary>
    /// <param name="filename">Զ���ļ���(Ҫע����׺)</param>
    /// <param name="localPath">Ҫ�洢��·��</param>
    /// <param name="action">�ص�ί�к���</param>
    public async void DownLoadHttp(string filename, string localPath, UnityAction<HttpStatusCode> action = null)
    {
        HttpStatusCode result = HttpStatusCode.OK;
        await Task.Run(() =>
        {
            try
            {
                //�ж��ļ��Ƿ���� Head 
                HttpWebRequest req = HttpWebRequest.Create(new Uri(HTTP_PATH + filename)) as HttpWebRequest;
                req.Method = WebRequestMethods.Http.Head;
                req.Timeout = 2000;
                //��������
                HttpWebResponse res = req.GetResponse() as HttpWebResponse;
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    res.Close();
                    //����
                    //����HTTP���Ӷ���
                    req = HttpWebRequest.Create(new Uri(HTTP_PATH + filename)) as HttpWebRequest;
                    req.Method = WebRequestMethods.Http.Get;
                    req.Timeout = 2000;
                    //��������
                    res = req.GetResponse() as HttpWebResponse;
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        //�洢����
                        using (FileStream fileStream = File.Create(localPath))
                        {
                            Stream downLoadStream = res.GetResponseStream();
                            byte[] bytes = new byte[2048];
                            int contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                            while (contentLength != 0)
                            {
                                fileStream.Write(bytes, 0, contentLength);
                                contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                            }
                            fileStream.Close();
                            downLoadStream.Close();
                        }
                        result = HttpStatusCode.OK;
                    }
                    else
                    {
                        result = res.StatusCode;
                    }
                }
                else
                {
                    result = res.StatusCode;
                }
                res.Close();
            }
            catch (WebException ex)
            {
                result = HttpStatusCode.InternalServerError;
                Debug.Log("���س���" + ex.Message + ex.Status);
            }
        });
        action?.Invoke(result);
    }
    /// <summary>
    /// ��Http�������ϴ��ļ�
    /// </summary>
    /// <param name="filename">Զ���ļ���(Ҫע����׺)</param>
    /// <param name="localPath">Ҫ�ϴ��ļ����ڵ�·��</param>
    /// <param name="action">�ص�ί�к���</param>
    public async void UpLoadHttp(string filename, string localPath, UnityAction<HttpStatusCode> action = null)
    {
        HttpStatusCode result = HttpStatusCode.BadRequest;
        await Task.Run(() =>
        {
            try
            {
                HttpWebRequest req = HttpWebRequest.Create(HTTP_PATH) as HttpWebRequest;
                //����
                req.Method = WebRequestMethods.Http.Post;
                req.ContentType = "multipart/form-data;boundary = LJP ";
                req.Timeout = 500000;
                //�����֤
                req.Credentials = new NetworkCredential(USER_NAME, PASS_WORD);
                req.PreAuthenticate = true;
                //һ��Ҫ�ǵÿ���
                //����ʽƴ���ַ���תΪ�ֽ�����
                string head = "--LJP\r\n" +
                    "Content-Disposition:form-data;name=\"file\";filename =\"{0}\"\r\n" +
                    "ContentType:application/octet-stream\r\n\r\n";
                //�滻�ļ���
                head = string.Format(head, filename);
                //ͷ��ƴ���ַ���������Ϣ������
                byte[] headBytes = Encoding.UTF8.GetBytes(head);
                byte[] endBytes = Encoding.UTF8.GetBytes("\r\n--LJP--\r\n");
                //д���ϴ���
                //�����ϴ�����
                //��д��ǰ����ͷ����Ϣ
                //��д���ļ�����
                //��д������ı߽���Ϣ
                //�ϴ����� ��ȡ��Ӧ
                using (FileStream localFileStream = File.OpenRead(localPath))
                {
                    //Ҫд����ܳ���
                    req.ContentLength = headBytes.Length + localFileStream.Length + endBytes.Length;
                    //�����ϴ�����
                    Stream upLoadStream = req.GetRequestStream();
                    upLoadStream.Write(headBytes, 0, headBytes.Length);
                    byte[] bytes = new byte[2048];
                    int contentLength = localFileStream.Read(bytes, 0, bytes.Length);
                    while (contentLength != 0)
                    {
                        upLoadStream.Write(bytes, 0, contentLength);
                        contentLength = localFileStream.Read(bytes, 0, bytes.Length);
                    }
                    upLoadStream.Write(endBytes, 0, endBytes.Length);
                    upLoadStream.Close();
                    localFileStream.Close();
                }
                //�ϴ�����
                HttpWebResponse res = req.GetResponse() as HttpWebResponse;
                result = res.StatusCode;
                res.Close();
            }
            catch (WebException e)
            {
                Debug.Log("�ϴ�����" + e.Message + e.Status);
            }
        });
        action?.Invoke(result);
    }
}
