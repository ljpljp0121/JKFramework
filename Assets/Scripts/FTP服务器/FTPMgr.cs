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

    //远端FTP服务器的地址
    private string FTP_PATH = "ftp://10.163.52.189/";
    //用户名和密码
    private string USER_NAME = "liaojianpeng";
    private string PASSWARD = "ljp2540297235";

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="fileName">FTP的文件名</param>
    /// <param name="localPath">本地文件路径</param>
    /// <param name="action">上传完毕后回调委托</param>
    public async void UpLoadFile(string fileName, string localPath, UnityAction action = null)
    {
        await Task.Run(() =>
        {
            try
            {
                //通过一个线程执行 逻辑 不会影响主线程
                //创建Ftp连接
                FtpWebRequest req = FtpWebRequest.Create(new Uri(FTP_PATH + fileName)) as FtpWebRequest;
                //设置
                //凭证
                req.Credentials = new NetworkCredential(USER_NAME, PASSWARD);
                //是否操作结束后关闭连接
                req.KeepAlive = false;
                //传输类型
                req.UseBinary = true;
                //操作类型
                req.Method = WebRequestMethods.Ftp.UploadFile;
                //代理置为空
                req.Proxy = null;
                //上传
                //用于上传的流对象
                Stream upLoadStream = req.GetRequestStream();
                using (FileStream fileStream = File.OpenRead(localPath))
                {
                    //可以一点一点的把文件中的字节数组读取出来 存入上传流中
                    byte[] bytes = new byte[1024];
                    //返回值 是从文件中读了多少个字节
                    int contentLength = fileStream.Read(bytes, 0, bytes.Length);
                    //不停读取文件中的字节 直到读完 
                    while (contentLength != 0)
                    {
                        //写入上传流中
                        upLoadStream.Write(bytes, 0, contentLength);
                        //写完了继续写
                        contentLength = fileStream.Read(bytes, 0, bytes.Length);
                    }
                    fileStream.Close();
                    upLoadStream.Close();
                    Debug.Log("上传成功");
                }
            }
            catch (Exception e)
            {
                Debug.Log("上传文件出错" + e.Message);
            }
        });
        //上传结束后的回调委托
        action?.Invoke();
    }
    /// <summary>
    /// 从服务器下载文件到本地
    /// </summary>
    /// <param name="fileName">ftp上的文件名</param>
    /// <param name="localPath">存储的本地文件路径</param>
    /// <param name="action">下载完毕回调委托函数</param>
    public async void DownLoadFile(string fileName, string localPath, UnityAction action = null)
    {
        await Task.Run(() =>
        {
            try
            {
                //创建一个连接
                FtpWebRequest req = FtpWebRequest.Create(new Uri(FTP_PATH + fileName)) as FtpWebRequest;
                //进行一些设置
                //凭证
                req.Credentials = new NetworkCredential(USER_NAME, PASSWARD);
                //是否操作结束后关闭控制连接
                req.KeepAlive = true;
                //代理设置为空
                req.Proxy = null;
                //操作类型 下载
                req.Method = WebRequestMethods.Ftp.DownloadFile;
                //传输类型
                req.UseBinary = true;
                //下载
                FtpWebResponse res = req.GetResponse() as FtpWebResponse;
                Stream downLoadStream = res.GetResponseStream();
                using (FileStream fileStream = File.Create(localPath))
                {
                    byte[] bytes = new byte[1024];
                    //读取数据
                    int contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                    //不断写入
                    while (contentLength != 0)
                    {
                        fileStream.Write(bytes, 0, contentLength);
                        contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                    }
                    fileStream.Close();
                    downLoadStream.Close();
                    Debug.Log("下载结束");
                }
                res.Close();
            }
            catch (Exception e)
            {
                Debug.Log("下载文件出错" + e.Message);
            }
        });
        action?.Invoke();
    }

    /// <summary>
    /// 移除指定文件
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="action">返回是否删除</param>
    public async void DeleteFile(string fileName, UnityAction<bool> action = null)
    {
        await Task.Run(() =>
        {
            try
            {
                //创建一个连接
                FtpWebRequest req = FtpWebRequest.Create(new Uri(FTP_PATH + fileName)) as FtpWebRequest;
                //进行一些设置
                //凭证
                req.Credentials = new NetworkCredential(USER_NAME, PASSWARD);
                //是否操作结束后关闭控制连接
                req.KeepAlive = true;
                //代理设置为空
                req.Proxy = null;
                //操作类型 删除
                req.Method = WebRequestMethods.Ftp.DeleteFile;
                //传输类型
                req.UseBinary = true;
                //真正的删除
                FtpWebResponse res = req.GetResponse() as FtpWebResponse;
                res.Close();
                action?.Invoke(true);
            }
            catch (Exception e)
            {
                Debug.Log("删除文件出错" + e.Message);
                action?.Invoke(false);
            }
        });
    }
    /// <summary>
    /// 返回FTP服务器上文件的大小/字节
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="action">返回文件大小</param>
    public async void GetFileSize(string fileName, UnityAction<long> action = null)
    {
        await Task.Run(() =>
        {
            try
            {
                //创建一个连接
                FtpWebRequest req = FtpWebRequest.Create(new Uri(FTP_PATH + fileName)) as FtpWebRequest;
                //进行一些设置
                //凭证
                req.Credentials = new NetworkCredential(USER_NAME, PASSWARD);
                //是否操作结束后关闭控制连接
                req.KeepAlive = true;
                //代理设置为空
                req.Proxy = null;
                //操作类型 获取大小
                req.Method = WebRequestMethods.Ftp.GetFileSize;
                //传输类型
                req.UseBinary = true;
                //真正的获取
                FtpWebResponse res = req.GetResponse() as FtpWebResponse;
                //把大小传递给外部
                action?.Invoke(res.ContentLength);
                res.Close();
            }
            catch (Exception e)
            {
                Debug.Log("获取文件大小出错" + e.Message);
            }
        });
    }

    /// <summary>
    /// 在FTP服务器上创建文件夹
    /// </summary>
    /// <param name="fileName">文件夹名称</param>
    /// <param name="action">创建完成的回调</param>
    public async void CreateDirectory(string directoryName, UnityAction<bool> action = null)
    {
        await Task.Run(() =>
        {
            try
            {
                //创建一个连接
                FtpWebRequest req = FtpWebRequest.Create(new Uri(FTP_PATH + directoryName)) as FtpWebRequest;
                //进行一些设置
                //凭证
                req.Credentials = new NetworkCredential(USER_NAME, PASSWARD);
                //是否操作结束后关闭控制连接
                req.KeepAlive = true;
                //代理设置为空
                req.Proxy = null;
                //操作类型 创建文件夹
                req.Method = WebRequestMethods.Ftp.MakeDirectory;
                //传输类型
                req.UseBinary = true;
                //真正的获取
                FtpWebResponse res = req.GetResponse() as FtpWebResponse;
                res.Close();
                action?.Invoke(true);
            }
            catch (Exception e)
            {
                Debug.Log("创建文件夹出错" + e.Message);
                action?.Invoke(false);
            }
        });
    }

    /// <summary>
    /// 在FTP服务器上获取文件夹的所有路径
    /// </summary>
    /// <param name="fileName">文件夹路径</param>
    /// <param name="action">供外部使用的文件名列表</param>
    public async void GetFileList(string directoryName, UnityAction<List<string>> action = null)
    {
        await Task.Run(() =>
        {
            try
            {
                //创建一个连接
                FtpWebRequest req = FtpWebRequest.Create(new Uri(FTP_PATH + directoryName)) as FtpWebRequest;
                //进行一些设置
                //凭证
                req.Credentials = new NetworkCredential(USER_NAME, PASSWARD);
                //是否操作结束后关闭控制连接
                req.KeepAlive = true;
                //代理设置为空
                req.Proxy = null;
                //操作类型 创建文件夹
                req.Method = WebRequestMethods.Ftp.ListDirectory;
                //传输类型
                req.UseBinary = true;
                //真正的获取
                FtpWebResponse res = req.GetResponse() as FtpWebResponse;
                //把下载的信息流转化为 StreamReader对象 方便一行一行的读取信息
                StreamReader streamReader = new StreamReader(res.GetResponseStream());
                //存储文件名的列表
                List<string> nameStrs = new List<string>();
                //一行行读取
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
                Debug.Log("创建文件夹出错" + e.Message);
                action?.Invoke(null);
            }
        });
    }
}
