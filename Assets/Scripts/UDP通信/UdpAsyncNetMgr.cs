using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class UdpAsyncNetMgr : MonoBehaviour
{
    private static UdpAsyncNetMgr instance;
    public static UdpAsyncNetMgr Instance => instance;

    private EndPoint serverIpPoint;

    private Socket socket;
    //socket是否关闭
    private bool isClose = true;

    //接受消息的队列 在多线程里面可以操作
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
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            //设置一个缓冲容器用于存放接受信息
            args.SetBuffer(cacheBytes, 0, cacheBytes.Length);
            //接受消息后会将发送方的ip和端口记入其中
            args.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            args.Completed += ReceiveMsg;
            socket.ReceiveFromAsync(args);
            print("客户端网络启动");
        }
        catch (Exception e)
        {
            print("启动socket出问题" + e.Message);
        }
    }
    //接受消息
    private void ReceiveMsg(object obj, SocketAsyncEventArgs args)
    {
        int nowIndex;
        int msgID;
        int msgLength;
        if (args.SocketError == SocketError.Success)
        {
            try
            {
                //如果是服务器的地址的话才处理
                if (args.RemoteEndPoint.Equals(serverIpPoint))
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
                //再次接受消息
                if (socket != null && !isClose)
                {
                    args.SetBuffer(0, cacheBytes.Length);
                    socket.ReceiveFromAsync(args);
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
        else
        {
            print("接受消息失败" + args.SocketError);
        }

    }
    //发送消息
    public void SendMsg(BaseInfo info)
    {
        try
        {
            if (socket != null && !isClose)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                byte[] bytes = info.WriteBytes();
                args.SetBuffer(bytes, 0, bytes.Length);
                args.Completed += SendToCallBack;
                //设置远端目标
                args.RemoteEndPoint = serverIpPoint;
                socket.SendToAsync(args);
            }
        }
        catch (SocketException e)
        {
            print("发送消息出错" + e.SocketErrorCode + e.Message);
        }
        catch (Exception ex)
        {
            print("发送消息出错(可能时序列化问题)" + ex.Message);
        }
    }

    private void SendToCallBack(object sender, SocketAsyncEventArgs e)
    {
        if (e.SocketError != SocketError.Success)
        {
            print("发送消息失败" + e.SocketError);
        }
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
