using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class WebLesson1 : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(UpLoad());
    }

    IEnumerator UpLoad()
    {
        //准备上传的数据
        List<IMultipartFormSection> data = new List<IMultipartFormSection>();

        data.Add(new MultipartFormDataSection("Name", "LJP"));
        //PlayerInfo playerInfo = new PlayerInfo();
        // data.Add(new MultipartFormDataSection("Info", playerInfo.WriteBytes()));

        //添加一些文件上传
        data.Add(new MultipartFormFileSection("Picture.jpg", File.ReadAllBytes(Application.streamingAssetsPath + "/pic1.jpg")));
        //传文本文件
        data.Add(new MultipartFormFileSection("12312312321", "Test123.txt"));

        UnityWebRequest req = UnityWebRequest.Post("ftp://10.163.52.189/", data);

        req.SendWebRequest();
        while (!req.isDone)
        {
            print(req.uploadProgress);
            print(req.uploadedBytes);
            yield return null;
        }
        print(req.uploadProgress);
        print(req.uploadedBytes);
        if (req.result == UnityWebRequest.Result.Success)
        {
            print("上传成功");
            //req.downloadHandler.data;
        }
        else
        {
            print("上传失败");
        }
    }

    void Update()
    {

    }
}
