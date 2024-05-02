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
        //׼���ϴ�������
        List<IMultipartFormSection> data = new List<IMultipartFormSection>();

        data.Add(new MultipartFormDataSection("Name", "LJP"));
        //PlayerInfo playerInfo = new PlayerInfo();
        // data.Add(new MultipartFormDataSection("Info", playerInfo.WriteBytes()));

        //���һЩ�ļ��ϴ�
        data.Add(new MultipartFormFileSection("Picture.jpg", File.ReadAllBytes(Application.streamingAssetsPath + "/pic1.jpg")));
        //���ı��ļ�
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
            print("�ϴ��ɹ�");
            //req.downloadHandler.data;
        }
        else
        {
            print("�ϴ�ʧ��");
        }
    }

    void Update()
    {

    }
}
