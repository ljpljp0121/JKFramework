using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class WWWFromTest : MonoBehaviour
{
    void Start()
    {

        //data.AddBinaryData();
        //data.AddField();
        StartCoroutine(UpLoadData());
    }

    IEnumerator UpLoadData()
    {
        WWWForm data = new WWWForm();
        data.AddField("Name", "LJP", Encoding.UTF8);
        data.AddField("Age", 99);

        data.AddBinaryData("file", File.ReadAllBytes(Application.streamingAssetsPath + "/pic1.jpg"));

        WWW www = new WWW("http://10.163.52.189:8080/HTTP_Server/", data);

        yield return www;

        if (www.error == null)
        {
            print("上传成功");
        }
        else
        {
            print("上传失败" + www.error);
        }

    }

    void Update()
    {

    }
}
