using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{    
    void Start()
    {
        if (NetManager.Instance == null)
        {
            GameObject obj = new GameObject("Net");
            obj.AddComponent<NetManager>();
        }

        NetManager.Instance.Connect("127.0.0.1", 8080);
    }

    void Update()
    {
        
    }
}
