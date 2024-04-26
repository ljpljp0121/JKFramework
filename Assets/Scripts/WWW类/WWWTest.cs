using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class WWWTest : MonoBehaviour
{
    public RawImage image;
    void Start()
    {
        if (NetWWWMgr.Instance == null)
        {
            GameObject obj = new GameObject("WWW");
            obj.AddComponent<NetWWWMgr>();
        }

        NetWWWMgr.Instance.LoadRes<Texture>("https://copyright.bdstatic.com/vcg/creative/cc9c744cf9f7c864889c563cbdeddce6.jpg@h_1280", (result) =>
        {
            image.texture = result;
        });
        NetWWWMgr.Instance.LoadRes<byte[]>("https://copyright.bdstatic.com/vcg/creative/cc9c744cf9f7c864889c563cbdeddce6.jpg@h_1280", (result) =>
        {
            File.WriteAllBytes(Application.persistentDataPath+ "/wwwͼƬ.jpg",result);
        });
    }

    void Update()
    {
        
    }
}
