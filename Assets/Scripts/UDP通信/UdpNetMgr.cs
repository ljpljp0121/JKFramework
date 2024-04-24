using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UdpNetMgr : MonoBehaviour
{
    private static UdpNetMgr instance;
    public static UdpNetMgr Instance => instance;

    private EndPoint serverIpPoint;

    private Socket socket;
    //socket是否关闭
    private bool isClose = true;

    //两个容器 队列 
    //接受 和发送消息的队列 在多线程里面可以操作
    private Queue<BaseInfo> sendQueue = new Queue<BaseInfo>();
    private Queue<BaseInfo> receiveQueue = new Queue<BaseInfo>();

    //接受消息的容器
    private byte[] cacheBytes = new byte[512];

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (receiveQueue.Count > 0)
        {
            BaseInfo info = receiveQueue.Dequeue();
            switch (info)
            {
                case PlayerInfo playerInfo:
                    print(playerInfo.playerID);
                    print(playerInfo.playerData.name);
                    print(playerInfo.playerData.atk);
                    print(playerInfo.playerData.lev);
                    print(playerInfo.playerData.sex);
                    break;
            }
        }
    }

    /// <summary>
    /// 启动客户端socket相关方法
    /// </summary>
    /// <param name="ip">远端服务器ip</param>
    /// <param name="port">远端服务器端口</param>
    public void StartClient(string ip, int port)
    {
        //开启状态不用再开
        if (!isClose)
        {
            return;
        }
        //先记录服务器地址
        serverIpPoint = new IPEndPoint(IPAddress.Parse(ip), port);

        IPEndPoint clientIpPort = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8081);
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(clientIpPort);
            isClose = false;
            print("客户端网络启动");
            ThreadPool.QueueUserWorkItem(ReceiveMsg);
            ThreadPool.QueueUserWorkItem(SendMsg);
        }
        catch (Exception e)
        {
            print("启动socket出问题" + e.Message);
        }
    }
    //接受消息
    private void ReceiveMsg(object obj)
    {
        EndPoint tempIpPoint = new IPEndPoint(IPAddress.Any, 0);
        int nowIndex;
        int msgID;
        int msgLength;
        while (!isClose)
        {
            if (socket != null && socket.Available > 0)
            {
                try
                {
                    socket.ReceiveFrom(cacheBytes, ref tempIpPoint);
                    //为了避免处理 非服务器发来的骚扰消息
                    if (!tempIpPoint.Equals(serverIpPoint))
                    {
                        continue;
                    }
                    else
                    {
                        //处理服务器发来的消息
                        nowIndex = 0;
                        //解析ID
                        msgID = BitConverter.ToInt32(cacheBytes, nowIndex);
                        nowIndex += 4;
                        //解析长度
                        msgLength = BitConverter.ToInt32(cacheBytes, nowIndex);
                        nowIndex += 4;
                        //解析消息体
                        BaseInfo info = null;
                        switch (msgID)
                        {
                            case 1001:
                                info = new PlayerInfo();
                                //反序列化消息体
                                info.ReadBytes(cacheBytes, nowIndex);
                                break;
                        }
                        if (info != null)
                        {
                            receiveQueue.Enqueue(info);
                        }
                    }
                }
                catch (SocketException e)
                {
                    print("接收消息出错" + e.SocketErrorCode + e.Message);
                }
                catch (Exception ex)
                {
                    print("接收消息出错(非网络问题)" + ex.Message);
                }
            }
        }
    }
    //发送消息线程
    private void SendMsg(object obj)
    {
        while (!isClose)
        {
            if (socket != null && sendQueue.Count > 0)
            {
                try
                {
                    socket.SendTo(sendQueue.Dequeue().WriteBytes(), serverIpPoint);
                }
                catch (SocketException e)
                {
                    print("发送消息出错" + e.SocketErrorCode + e.Message);
                }
            }
        }
    }
    //供外部调用的发送消息方法 将消息压入队列容器中
    public void Send(BaseInfo info)
    {
        sendQueue.Enqueue(info);
    }

    //关闭socket
    public void Close()
    {
        if (socket != null)
        {
            isClose = true;
            //发送退出消息给服务器
            QuitMsg quitMsg = new QuitMsg();
            socket.SendTo(quitMsg.WriteBytes(), serverIpPoint);
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }

    private void OnDestroy()
    {
        Close();
    }
}
