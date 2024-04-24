using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAsyncUdp : MonoBehaviour
{    
    void Start()
    {
        if (UdpAsyncNetMgr.Instance == null)
        {
            GameObject obj = new GameObject("UdpAsyncNet");
            obj.AddComponent<UdpAsyncNetMgr>();
        }

        UdpAsyncNetMgr.Instance.StartClient("127.0.0.1", 8080);
    }

    void Update()
    {
        
    }
}
