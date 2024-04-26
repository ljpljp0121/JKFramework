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
        //创建HttpWebRequest对象
        HttpWebRequest req = HttpWebRequest.Create("http://10.163.52.189:8080/HTTP_Server/") as HttpWebRequest;
        //设置
        req.Method = WebRequestMethods.Http.Post;
        req.ContentType = "multipart/form-data;boundary = LJP ";
        req.Timeout = 500000;
        //身份验证
        req.Credentials = new NetworkCredential("liaojianepng", "ljp2540297235");
        req.PreAuthenticate = true;
        //一定要记得空行
        //按格式拼接字符串转为字节数组
        string head = "--LJP\r\n" +
            "Content-Disposition:form-data;name=\"file\";filename =\"HTTP上传文件.jpg\"\r\n" +
            "ContentType:application/octet-stream\r\n\r\n";
        //头部拼接字符串规则信息的数组
        byte[] headBytes = Encoding.UTF8.GetBytes(head);
        byte[] endBytes = Encoding.UTF8.GetBytes("\r\n--边界字符串--\r\n");
        //写入上传流
        //设置上传长度
        //先写入前部分头部信息
        //再写入文件数据
        //在写入结束的边界信息
        //上传数据 获取响应
        using (FileStream localFileStream = File.OpenRead(Application.streamingAssetsPath + "/pic1.jpg"))
        {
            //要写入的总长度
            req.ContentLength = headBytes.Length + localFileStream.Length + endBytes.Length;
            //用于上传的流
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
        //上传数据
        HttpWebResponse res = req.GetResponse() as HttpWebResponse;
        if (res.StatusCode == HttpStatusCode.OK)
        {
            print("上传通信成功");
        }
        else
        {
            print("上传失败" + res.StatusCode);
        }

    }

    void Update()
    {

    }
}
