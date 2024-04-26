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

    //远端HTTP服务器的地址
    private string HTTP_PATH = "http://10.163.52.189:8080/HTTP_Server/";

    private string USER_NAME = "liaojianpeng";
    private string PASS_WORD = "ljp2540292735";

    /// <summary>
    /// 从Http服务器下载文件
    /// </summary>
    /// <param name="filename">远程文件名(要注明后缀)</param>
    /// <param name="localPath">要存储的路径</param>
    /// <param name="action">回调委托函数</param>
    public async void DownLoadHttp(string filename, string localPath, UnityAction<HttpStatusCode> action = null)
    {
        HttpStatusCode result = HttpStatusCode.OK;
        await Task.Run(() =>
        {
            try
            {
                //判断文件是否存在 Head 
                HttpWebRequest req = HttpWebRequest.Create(new Uri(HTTP_PATH + filename)) as HttpWebRequest;
                req.Method = WebRequestMethods.Http.Head;
                req.Timeout = 2000;
                //发送请求
                HttpWebResponse res = req.GetResponse() as HttpWebResponse;
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    res.Close();
                    //下载
                    //创建HTTP连接对象
                    req = HttpWebRequest.Create(new Uri(HTTP_PATH + filename)) as HttpWebRequest;
                    req.Method = WebRequestMethods.Http.Get;
                    req.Timeout = 2000;
                    //发送请求
                    res = req.GetResponse() as HttpWebResponse;
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        //存储数据
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
                Debug.Log("下载出错" + ex.Message + ex.Status);
            }
        });
        action?.Invoke(result);
    }
    /// <summary>
    /// 向Http服务器上传文件
    /// </summary>
    /// <param name="filename">远程文件名(要注明后缀)</param>
    /// <param name="localPath">要上传文件所在的路径</param>
    /// <param name="action">回调委托函数</param>
    public async void UpLoadHttp(string filename, string localPath, UnityAction<HttpStatusCode> action = null)
    {
        HttpStatusCode result = HttpStatusCode.BadRequest;
        await Task.Run(() =>
        {
            try
            {
                HttpWebRequest req = HttpWebRequest.Create(HTTP_PATH) as HttpWebRequest;
                //设置
                req.Method = WebRequestMethods.Http.Post;
                req.ContentType = "multipart/form-data;boundary = LJP ";
                req.Timeout = 500000;
                //身份验证
                req.Credentials = new NetworkCredential(USER_NAME, PASS_WORD);
                req.PreAuthenticate = true;
                //一定要记得空行
                //按格式拼接字符串转为字节数组
                string head = "--LJP\r\n" +
                    "Content-Disposition:form-data;name=\"file\";filename =\"{0}\"\r\n" +
                    "ContentType:application/octet-stream\r\n\r\n";
                //替换文件名
                head = string.Format(head, filename);
                //头部拼接字符串规则信息的数组
                byte[] headBytes = Encoding.UTF8.GetBytes(head);
                byte[] endBytes = Encoding.UTF8.GetBytes("\r\n--LJP--\r\n");
                //写入上传流
                //设置上传长度
                //先写入前部分头部信息
                //再写入文件数据
                //在写入结束的边界信息
                //上传数据 获取响应
                using (FileStream localFileStream = File.OpenRead(localPath))
                {
                    //要写入的总长度
                    req.ContentLength = headBytes.Length + localFileStream.Length + endBytes.Length;
                    //用于上传的流
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
                //上传数据
                HttpWebResponse res = req.GetResponse() as HttpWebResponse;
                result = res.StatusCode;
                res.Close();
            }
            catch (WebException e)
            {
                Debug.Log("上传出错" + e.Message + e.Status);
            }
        });
        action?.Invoke(result);
    }
}
