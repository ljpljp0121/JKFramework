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
        //检测资源可用性
        try
        {

            //创建HTTP通讯用连接对象
            HttpWebRequest req = HttpWebRequest.Create(new Uri("http://10.163.52.189:8080/HTTP_Server/est.txt")) as HttpWebRequest;

            //设置请求类型 或其它参数
            req.Method = WebRequestMethods.Http.Head;
            //超时
            req.Timeout = 3000;

            //发送请求
            HttpWebResponse res = req.GetResponse() as HttpWebResponse;

            if (res.StatusCode == HttpStatusCode.OK)
            {
                print("文件存在");
                print(res.ContentLength);
                print(res.ContentType);
                res.Close();
            }
            else
            {
                print("文件不可用" + res.StatusCode);
            }
        }
        catch (WebException ex)
        {
            print("获取出错" + ex.Message + ex.Status);
        }

        //下载资源
        try
        {
            HttpWebRequest req = HttpWebRequest.Create(new Uri("http://10.163.52.189:8080/HTTP_Server/est.txt")) as HttpWebRequest;
            //设置请求类型 或其它参数
            req.Method = WebRequestMethods.Http.Get;
            req.Timeout = 3000;
            //发送请求
            HttpWebResponse res = req.GetResponse() as HttpWebResponse;
            //获取相应数据流 写入本地路径
            if (res.StatusCode == HttpStatusCode.OK)
            {
                print(Application.persistentDataPath);
                using (FileStream fileStream = File.Create(Application.persistentDataPath + "/httpDownLoad.txt"))
                {
                    Stream downLoadStream = res.GetResponseStream();
                    byte[] bytes = new byte[2048];
                    int contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                    //不断写入
                    while (contentLength != 0)
                    {
                        fileStream.Write(bytes, 0, contentLength);
                        contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                    }
                    fileStream.Close();
                    downLoadStream.Close();
                    res.Close();
                }
                print("下载结束");
            }
            else
            {
                print("下载失败" + res.StatusCode);
            }
        }
        catch (WebException ex)
        {
            print("下载出错" + ex.Message + ex.Status);
        }
    }

    void Update()
    {

    }
}
