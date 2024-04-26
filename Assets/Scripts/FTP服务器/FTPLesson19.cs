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
        //    Debug.Log("上传成功");
        //});
        //FTPMgr.Instance.DownLoadFile(fileName, Application.persistentDataPath+"/ljp.jpg", () =>
        //{
        //    Debug.Log("下载成功");
        //});
        //FTPMgr.Instance.DeleteFile("Test.txt");
        //FTPMgr.Instance.GetFileSize("pic.png", (result) =>
        //{
        //    print("文件大小为" + result);
        //});
        //FTPMgr.Instance.CreateDirectory("廖建鹏", (result) =>
        //{
        //    print(result? "创建成功":"创建失败");
        //});
        FTPMgr.Instance.GetFileList("", (list) =>
        {
            if (list == null)
            {
                print("获取文件失败");
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
