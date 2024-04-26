using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class FTPMgr
{
    private static FTPMgr instance = new FTPMgr();
    public static FTPMgr Instance => instance;

    //Զ��FTP�������ĵ�ַ
    private string FTP_PATH = "ftp://10.163.52.189/";
    //�û���������
    private string USER_NAME = "liaojianpeng";
    private string PASSWARD = "ljp2540297235";

    /// <summary>
    /// �ϴ��ļ�
    /// </summary>
    /// <param name="fileName">FTP���ļ���</param>
    /// <param name="localPath">�����ļ�·��</param>
    /// <param name="action">�ϴ���Ϻ�ص�ί��</param>
    public async void UpLoadFile(string fileName, string localPath, UnityAction action = null)
    {
        await Task.Run(() =>
        {
            try
            {
                //ͨ��һ���߳�ִ�� �߼� ����Ӱ�����߳�
                //����Ftp����
                FtpWebRequest req = FtpWebRequest.Create(new Uri(FTP_PATH + fileName)) as FtpWebRequest;
                //����
                //ƾ֤
                req.Credentials = new NetworkCredential(USER_NAME, PASSWARD);
                //�Ƿ����������ر�����
                req.KeepAlive = false;
                //��������
                req.UseBinary = true;
                //��������
                req.Method = WebRequestMethods.Ftp.UploadFile;
                //������Ϊ��
                req.Proxy = null;
                //�ϴ�
                //�����ϴ���������
                Stream upLoadStream = req.GetRequestStream();
                using (FileStream fileStream = File.OpenRead(localPath))
                {
                    //����һ��һ��İ��ļ��е��ֽ������ȡ���� �����ϴ�����
                    byte[] bytes = new byte[1024];
                    //����ֵ �Ǵ��ļ��ж��˶��ٸ��ֽ�
                    int contentLength = fileStream.Read(bytes, 0, bytes.Length);
                    //��ͣ��ȡ�ļ��е��ֽ� ֱ������ 
                    while (contentLength != 0)
                    {
                        //д���ϴ�����
                        upLoadStream.Write(bytes, 0, contentLength);
                        //д���˼���д
                        contentLength = fileStream.Read(bytes, 0, bytes.Length);
                    }
                    fileStream.Close();
                    upLoadStream.Close();
                    Debug.Log("�ϴ��ɹ�");
                }
            }
            catch (Exception e)
            {
                Debug.Log("�ϴ��ļ�����" + e.Message);
            }
        });
        //�ϴ�������Ļص�ί��
        action?.Invoke();
    }
    /// <summary>
    /// �ӷ����������ļ�������
    /// </summary>
    /// <param name="fileName">ftp�ϵ��ļ���</param>
    /// <param name="localPath">�洢�ı����ļ�·��</param>
    /// <param name="action">������ϻص�ί�к���</param>
    public async void DownLoadFile(string fileName, string localPath, UnityAction action = null)
    {
        await Task.Run(() =>
        {
            try
            {
                //����һ������
                FtpWebRequest req = FtpWebRequest.Create(new Uri(FTP_PATH + fileName)) as FtpWebRequest;
                //����һЩ����
                //ƾ֤
                req.Credentials = new NetworkCredential(USER_NAME, PASSWARD);
                //�Ƿ����������رտ�������
                req.KeepAlive = true;
                //��������Ϊ��
                req.Proxy = null;
                //�������� ����
                req.Method = WebRequestMethods.Ftp.DownloadFile;
                //��������
                req.UseBinary = true;
                //����
                FtpWebResponse res = req.GetResponse() as FtpWebResponse;
                Stream downLoadStream = res.GetResponseStream();
                using (FileStream fileStream = File.Create(localPath))
                {
                    byte[] bytes = new byte[1024];
                    //��ȡ����
                    int contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                    //����д��
                    while (contentLength != 0)
                    {
                        fileStream.Write(bytes, 0, contentLength);
                        contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                    }
                    fileStream.Close();
                    downLoadStream.Close();
                    Debug.Log("���ؽ���");
                }
                res.Close();
            }
            catch (Exception e)
            {
                Debug.Log("�����ļ�����" + e.Message);
            }
        });
        action?.Invoke();
    }

    /// <summary>
    /// �Ƴ�ָ���ļ�
    /// </summary>
    /// <param name="fileName">�ļ���</param>
    /// <param name="action">�����Ƿ�ɾ��</param>
    public async void DeleteFile(string fileName, UnityAction<bool> action = null)
    {
        await Task.Run(() =>
        {
            try
            {
                //����һ������
                FtpWebRequest req = FtpWebRequest.Create(new Uri(FTP_PATH + fileName)) as FtpWebRequest;
                //����һЩ����
                //ƾ֤
                req.Credentials = new NetworkCredential(USER_NAME, PASSWARD);
                //�Ƿ����������رտ�������
                req.KeepAlive = true;
                //��������Ϊ��
                req.Proxy = null;
                //�������� ɾ��
                req.Method = WebRequestMethods.Ftp.DeleteFile;
                //��������
                req.UseBinary = true;
                //������ɾ��
                FtpWebResponse res = req.GetResponse() as FtpWebResponse;
                res.Close();
                action?.Invoke(true);
            }
            catch (Exception e)
            {
                Debug.Log("ɾ���ļ�����" + e.Message);
                action?.Invoke(false);
            }
        });
    }
    /// <summary>
    /// ����FTP���������ļ��Ĵ�С/�ֽ�
    /// </summary>
    /// <param name="fileName">�ļ���</param>
    /// <param name="action">�����ļ���С</param>
    public async void GetFileSize(string fileName, UnityAction<long> action = null)
    {
        await Task.Run(() =>
        {
            try
            {
                //����һ������
                FtpWebRequest req = FtpWebRequest.Create(new Uri(FTP_PATH + fileName)) as FtpWebRequest;
                //����һЩ����
                //ƾ֤
                req.Credentials = new NetworkCredential(USER_NAME, PASSWARD);
                //�Ƿ����������رտ�������
                req.KeepAlive = true;
                //��������Ϊ��
                req.Proxy = null;
                //�������� ��ȡ��С
                req.Method = WebRequestMethods.Ftp.GetFileSize;
                //��������
                req.UseBinary = true;
                //�����Ļ�ȡ
                FtpWebResponse res = req.GetResponse() as FtpWebResponse;
                //�Ѵ�С���ݸ��ⲿ
                action?.Invoke(res.ContentLength);
                res.Close();
            }
            catch (Exception e)
            {
                Debug.Log("��ȡ�ļ���С����" + e.Message);
            }
        });
    }

    /// <summary>
    /// ��FTP�������ϴ����ļ���
    /// </summary>
    /// <param name="fileName">�ļ�������</param>
    /// <param name="action">������ɵĻص�</param>
    public async void CreateDirectory(string directoryName, UnityAction<bool> action = null)
    {
        await Task.Run(() =>
        {
            try
            {
                //����һ������
                FtpWebRequest req = FtpWebRequest.Create(new Uri(FTP_PATH + directoryName)) as FtpWebRequest;
                //����һЩ����
                //ƾ֤
                req.Credentials = new NetworkCredential(USER_NAME, PASSWARD);
                //�Ƿ����������رտ�������
                req.KeepAlive = true;
                //��������Ϊ��
                req.Proxy = null;
                //�������� �����ļ���
                req.Method = WebRequestMethods.Ftp.MakeDirectory;
                //��������
                req.UseBinary = true;
                //�����Ļ�ȡ
                FtpWebResponse res = req.GetResponse() as FtpWebResponse;
                res.Close();
                action?.Invoke(true);
            }
            catch (Exception e)
            {
                Debug.Log("�����ļ��г���" + e.Message);
                action?.Invoke(false);
            }
        });
    }

    /// <summary>
    /// ��FTP�������ϻ�ȡ�ļ��е�����·��
    /// </summary>
    /// <param name="fileName">�ļ���·��</param>
    /// <param name="action">���ⲿʹ�õ��ļ����б�</param>
    public async void GetFileList(string directoryName, UnityAction<List<string>> action = null)
    {
        await Task.Run(() =>
        {
            try
            {
                //����һ������
                FtpWebRequest req = FtpWebRequest.Create(new Uri(FTP_PATH + directoryName)) as FtpWebRequest;
                //����һЩ����
                //ƾ֤
                req.Credentials = new NetworkCredential(USER_NAME, PASSWARD);
                //�Ƿ����������رտ�������
                req.KeepAlive = true;
                //��������Ϊ��
                req.Proxy = null;
                //�������� �����ļ���
                req.Method = WebRequestMethods.Ftp.ListDirectory;
                //��������
                req.UseBinary = true;
                //�����Ļ�ȡ
                FtpWebResponse res = req.GetResponse() as FtpWebResponse;
                //�����ص���Ϣ��ת��Ϊ StreamReader���� ����һ��һ�еĶ�ȡ��Ϣ
                StreamReader streamReader = new StreamReader(res.GetResponseStream());
                //�洢�ļ������б�
                List<string> nameStrs = new List<string>();
                //һ���ж�ȡ
                string line = streamReader.ReadLine();
                while (line != null)
                {
                    nameStrs.Add(line);
                    line = streamReader.ReadLine();
                }
                res.Close();
                action?.Invoke(nameStrs);
            }
            catch (Exception e)
            {
                Debug.Log("�����ļ��г���" + e.Message);
                action?.Invoke(null);
            }
        });
    }
}
