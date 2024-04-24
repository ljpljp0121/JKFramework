using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
public class Socket_2 : MonoBehaviour
{
    public Button button;
    public Button button1;
    public Button button2;
    public Button button3;


    public InputField inputField;
    void Start()
    {
        button.onClick.AddListener(() =>
        {
            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.playerID = 1;
            playerInfo.playerData = new PlayerData();
            playerInfo.playerData.name = "廖建鹏";
            playerInfo.playerData.lev = 22;
            playerInfo.playerData.atk = 99;
            NetManager.Instance.Send(playerInfo);
        });
        //粘包测试
        button1.onClick.AddListener(() =>
        {
            PlayerInfo playerInfo1 = new PlayerInfo();
            playerInfo1.playerID = 1001;
            playerInfo1.playerData = new PlayerData();
            playerInfo1.playerData.name = "廖建鹏第一次";
            playerInfo1.playerData.lev = 22;
            playerInfo1.playerData.atk = 99;

            PlayerInfo playerInfo2 = new PlayerInfo();
            playerInfo2.playerID = 1002;
            playerInfo2.playerData = new PlayerData();
            playerInfo2.playerData.name = "廖建鹏第二次";
            playerInfo2.playerData.lev = 11;
            playerInfo2.playerData.atk = 999;
            //粘包
            byte[] bytes = new byte[playerInfo1.GetBytesNum() + playerInfo2.GetBytesNum()];
            playerInfo1.WriteBytes().CopyTo(bytes, 0);
            playerInfo2.WriteBytes().CopyTo(bytes, playerInfo1.GetBytesNum());
            NetManager.Instance.SendTest(bytes);
        });
        //分包测试
        button2.onClick.AddListener(() =>
        {
            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.playerID = 1003;
            playerInfo.playerData = new PlayerData();
            playerInfo.playerData.name = "廖建鹏";
            playerInfo.playerData.lev = 22;
            playerInfo.playerData.atk = 99;

            byte[] bytes = playerInfo.WriteBytes();
            //分包
            byte[] bytes1 = new byte[10];
            byte[] bytes2 = new byte[bytes.Length - 10];
            Array.Copy(bytes, 0, bytes1, 0, 10);
            Array.Copy(bytes, 10, bytes2, 0, bytes.Length - 10);
            NetManager.Instance.SendTest(bytes1);
            NetManager.Instance.SendTest(bytes2);
        });
        //分包粘包测试
        button3.onClick.AddListener(async () =>
        {
            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.playerID = 1003;
            playerInfo.playerData = new PlayerData();
            playerInfo.playerData.name = "廖建鹏";
            playerInfo.playerData.lev = 22;
            playerInfo.playerData.atk = 99;

            PlayerInfo playerInfo1 = new PlayerInfo();
            playerInfo1.playerID = 1001;
            playerInfo1.playerData = new PlayerData();
            playerInfo1.playerData.name = "廖建鹏";
            playerInfo1.playerData.lev = 21;
            playerInfo1.playerData.atk = 92;


            byte[] bytes1 = playerInfo.WriteBytes();
            byte[] bytes2 = playerInfo1.WriteBytes();

            byte[] bytes2_1 = new byte[10];
            byte[] bytes2_2 = new byte[bytes2.Length - 10];

            Array.Copy(bytes1, 0, bytes2_1, 0, 10);
            Array.Copy(bytes2, 10, bytes2_2, 0, bytes2.Length - 10);

            byte[] bytes = new byte[bytes1.Length + bytes2_1.Length];
            bytes1.CopyTo(bytes, 0);
            bytes2_1.CopyTo(bytes, bytes1.Length);

            NetManager.Instance.SendTest(bytes);
            await Task.Delay(1000);
            NetManager.Instance.SendTest(bytes2_2);
        });
    }


    void Update()
    {

    }
}
