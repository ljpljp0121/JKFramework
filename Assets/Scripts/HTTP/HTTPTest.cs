using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HTTPTest : MonoBehaviour
{
    void Start()
    {

        HTTPMgr.Instance.DownLoadHttp("est.txt", Application.persistentDataPath + "/Test.txt", (result) =>
        {
            if (result == System.Net.HttpStatusCode.OK)
            {
                Debug.Log("下载成功");
            }
            else
            {
                Debug.Log( "下载失败"+result);
            }
        });
        HTTPMgr.Instance.UpLoadHttp("Test.jpg", Application.streamingAssetsPath + "/pic1.jpg");
    }

    void Update()
    {

    }
}
