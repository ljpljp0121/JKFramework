using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WWWLesson1 : MonoBehaviour
{
    public RawImage image;
    void Start()
    {
        //www.GetAudioClip();
        //Texture2D tex = new Texture2D(100, 100);
        //www.LoadImageIntoTexture(tex);
        //WWW.LoadFromCacheOrDownload("http://10.163.52.189:8080/HTTP_Server/Test.jpg",1);

        //www.assetBundle
        //www.GetAudioClip()
        //www.bytes
        //www.bytesDownloaded
        //www.error
        //www.isDone

        //下载http服务器内容
        //StartCoroutine(DownLoadHttp());
        //StartCoroutine(DownLoadFtp());
        StartCoroutine(DownLoadLocal());
    }

    IEnumerator DownLoadHttp()
    {
        //创建www对象
        WWW www = new WWW("https://copyright.bdstatic.com/vcg/creative/cc9c744cf9f7c864889c563cbdeddce6.jpg@h_1280");

        while(!www.isDone)
        {
            print(www.bytesDownloaded);
            print(www.progress);
            yield return null;
        }

        print(www.bytesDownloaded);
        print(www.progress);
        //使用加载结束后的资源
        if (www.error == null)
        {
           image.texture = www.texture;
        }
        else
        {
            print(www.error); 
        }
    }
    
    IEnumerator DownLoadFtp()
    {
        //创建www对象
        WWW www = new WWW("ftp://10.163.52.189/picture.jpg");

        while (!www.isDone)
        {
            print(www.bytesDownloaded);
            print(www.progress);
            yield return null;
        }

        print(www.bytesDownloaded);
        print(www.progress);
        //使用加载结束后的资源
        if (www.error == null)
        {
            image.texture = www.texture;
        }
        else
        {
            print(www.error);
        }
    }

    IEnumerator DownLoadLocal()
    {
        //创建www对象
        WWW www = new WWW("file://" + Application.streamingAssetsPath + "/pic1.jpg");

        while (!www.isDone)
        {
            print(www.bytesDownloaded);
            print(www.progress);
            yield return null;
        }

        print(www.bytesDownloaded);
        print(www.progress);
        //使用加载结束后的资源
        if (www.error == null)
        {
            image.texture = www.texture;
        }
        else
        {
            print(www.error);
        }
    }

    void Update()
    {
        
    }
}
