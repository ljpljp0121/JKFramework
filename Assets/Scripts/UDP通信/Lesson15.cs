using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lesson15 : MonoBehaviour
{
    public Button button;
    void Start()
    {
        button.onClick.AddListener(() =>
        {
            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.playerData = new PlayerData();
            playerInfo.playerID = 1;
            playerInfo.playerData.name = "¡ŒΩ®≈Ù";
            playerInfo.playerData.lev = 11;
            playerInfo.playerData.atk =22;
            playerInfo.playerData.sex = false;
            UdpAsyncNetMgr.Instance.SendMsg(playerInfo);
        });
    }

    void Update()
    {
        
    }
}
