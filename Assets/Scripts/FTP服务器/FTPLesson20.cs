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
            //设置操作命令 下载文件
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            req.UseBinary = true;
            //用于下载的流对象
            FtpWebResponse res = req.GetResponse() as FtpWebResponse;
            Stream downLoadStream = res.GetResponseStream();

            //开始下载
            print(Application.persistentDataPath);
            using (FileStream fileStream = File.Create(Application.persistentDataPath + "/picture111.jpg"))
            {

                byte[] bytes = new byte[1024];
                //读取下载下来的流数据
                int contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                //不停读取文件中的字节 直到读完 
                while (contentLength != 0)
                {
                    //写入本地文件流
                    fileStream.Write(bytes, 0, contentLength);
                    //写完了继续写
                    contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                }
                fileStream.Close();
                downLoadStream.Close();
                print("上传结束");
            }
        }
        catch (Exception e)
        {
            print("下载出错" + e.Message);
        }
    }

    void Update()
    {

    }
}
