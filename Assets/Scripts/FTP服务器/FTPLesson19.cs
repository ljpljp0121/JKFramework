using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class FTPLesson19 : MonoBehaviour
{
    void Start()
    {
        string fileName = "picture.jpg";
        string localPath = Application.streamingAssetsPath + "/pic1.jpg";
        //FTPMgr.Instance.UpLoadFile(fileName, localPath, () =>
        //{
        //    Debug.Log("�ϴ��ɹ�");
        //});
        //FTPMgr.Instance.DownLoadFile(fileName, Application.persistentDataPath+"/ljp.jpg", () =>
        //{
        //    Debug.Log("���سɹ�");
        //});
        //FTPMgr.Instance.DeleteFile("Test.txt");
        //FTPMgr.Instance.GetFileSize("pic.png", (result) =>
        //{
        //    print("�ļ���СΪ" + result);
        //});
        //FTPMgr.Instance.CreateDirectory("�ν���", (result) =>
        //{
        //    print(result? "�����ɹ�":"����ʧ��");
        //});
        FTPMgr.Instance.GetFileList("", (list) =>
        {
            if (list == null)
            {
                print("��ȡ�ļ�ʧ��");
                return;
            }
            else
            {
                foreach (string file in list)
                {
                    Debug.Log(file);
                }
            }
        });
    }

    void Update()
    {

    }
}
