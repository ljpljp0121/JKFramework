using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUdp : MonoBehaviour
{    
    void Start()
    {
        if (UdpNetMgr.Instance == null)
        {
            GameObject obj = new GameObject("UdpNet");
            obj.AddComponent<UdpNetMgr>();
        }

        UdpNetMgr.Instance.StartClient("127.0.0.1", 8080);
    }

    void Update()
    {
        
    }
}
