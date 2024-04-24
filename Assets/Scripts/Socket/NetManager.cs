using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class NetManager : MonoBehaviour
{
    private static NetManager instance;
    public static NetManager Instance => instance;

    //客户端Socket
    private Socket socket;
    //用于发送消息的队列，主线程往里面放 其他线程取
    private Queue<BaseInfo> sendMsgQueue = new Queue<BaseInfo>();
    //用于接受消息的队列 子线程往里放 主线程来取
    private Queue<BaseInfo> receiveQueue = new Queue<BaseInfo>();


    ////用于收消息的容器
    //private byte[] receiveBytes = new byte[1024 * 1024];
    ////返回收到的字节数
    //private int receiveNum;

    //缓存的字节数组和长度 处理分包
    private byte[] cacheBytes = new byte[1024 * 1024];
    private int cacheNum = 0;

    //是否连接
    private bool isConnected = false;

    private int SEND_HEART_MSG_TIME = 2;
    private Heart heart = new Heart();

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

        //定时循环发送心跳消息
        InvokeRepeating("SendHeartMsg", 0, SEND_HEART_MSG_TIME);
    }

    private void SendHeartMsg()
    {
        if (isConnected)
            Send(heart);
    }

    void Update()
    {
        if (receiveQueue.Count > 0)
        {
            BaseInfo info = receiveQueue.Dequeue();
            if (info is PlayerInfo)
            {
                PlayerInfo playerInfo = (PlayerInfo)info;
                print(playerInfo.playerID);
                print(playerInfo.playerData.name);
                print(playerInfo.playerData.lev);
                print(playerInfo.playerData.atk);
            }
        }
    }

    //连接服务端
    public void Connect(string ip, int port)
    {
        //如果是连接状态 直接返回
        if (isConnected)
        {
            return;
        }
        if (socket == null)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        //连接服务端
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        try
        {
            socket.Connect(ipPoint);
            isConnected = true;
            //开启发送线程
            ThreadPool.QueueUserWorkItem(SendMsg);
            //sendThread = new Thread(SendMsg);
            //sendThread.Start();
            //开启接受线程
            ThreadPool.QueueUserWorkItem(ReceiveMsg);
            //receiveThread = new Thread(ReceiveMsg);
            //receiveThread.Start();
        }
        catch (SocketException e)
        {
            if (e.ErrorCode == 10061)
                print("服务器拒绝连接");
            print("连接失败" + e.ErrorCode + e.Message);
        }
    }
    //发送消息
    public void Send(BaseInfo info)
    {
        sendMsgQueue.Enqueue(info);
    }
    /// <summary>
    /// 用于测试 直接发字节数组
    /// </summary>
    /// <param name="bytes"></param>
    public void SendTest(byte[] bytes)
    {
        socket.Send(bytes);
    }

    //多线程 从消息队列发送消息
    private void SendMsg(object obj)
    {
        while (isConnected)
        {
            if (sendMsgQueue.Count > 0)
            {
                socket.Send(sendMsgQueue.Dequeue().WriteBytes());
            }
        }
    }
    //不断接受消息
    private void ReceiveMsg(object obj)
    {
        while (isConnected)
        {
            if (socket.Available > 0)
            {
                byte[] receiveBytes = new byte[1024 * 1024];
                int receiveNum = socket.Receive(receiveBytes);
                HandleReceiveMsg(receiveBytes, receiveNum);
                ////首先解析消息ID
                ////使用字节数组前四个字节得到ID
                //int msgID = BitConverter.ToInt32(receiveBytes, 0);
                //BaseInfo baseInfo = null;
                //switch (msgID)
                //{
                //    case 1001:
                //        PlayerInfo playerInfo = new PlayerInfo();
                //        playerInfo.ReadBytes(receiveBytes,4);
                //        baseInfo = playerInfo;
                //        break;
                //}
                ////如果消息为空 没有解析
                //if (baseInfo == null)
                //    continue;
                ////收到消息 解析为字符串 放入容器
                //receiveQueue.Enqueue(baseInfo);
            }
        }
    }
    //处理消息 解决分包 粘包
    private void HandleReceiveMsg(byte[] receiveBytes, int receiveNum)
    {
        int msgID = 0;
        int msgLength = 0;
        int nowIndex = 0;

        //收到消息是查看 缓存是否为空 有则拼接到后面
        receiveBytes.CopyTo(cacheBytes, cacheNum);
        cacheNum += receiveNum;

        while (true)
        {
            //每次将长度设置为-1 避免上一次解析数据影响这一次判断
            msgLength = -1;
            //处理解析一条消息
            if (cacheNum - nowIndex >= 8)
            {
                //解析ID
                msgID = BitConverter.ToInt32(cacheBytes, nowIndex);
                nowIndex += 4;
                //解析长度
                msgLength = BitConverter.ToInt32(cacheBytes, nowIndex);
                nowIndex += 4;
            }
            //长度大于Length才解析
            if (cacheNum - nowIndex >= msgLength && msgLength != -1)
            {
                //解析消息体
                BaseInfo baseInfo = null;
                switch (msgID)
                {
                    case 1001:
                        PlayerInfo playerInfo = new PlayerInfo();
                        playerInfo.ReadBytes(cacheBytes, nowIndex);
                        baseInfo = playerInfo;
                        break;
                }
                if (baseInfo != null)
                    receiveQueue.Enqueue(baseInfo);
                nowIndex += msgLength;
                if (nowIndex == cacheNum)
                {
                    cacheNum = 0;
                    break;
                }
            }
            else
            {
                //还是不满足说明有分包，
                //要把它保存下来下一次接收到消息将其拼接起来 再做处理
                //receiveBytes.CopyTo(cacheBytes, 0);
                //cacheNum = receiveNum;
                //如果进行了id和长度解析 但是没有成功解析消息体 那么需要减去nowIndex移动位置
                if (msgLength != -1)
                {
                    nowIndex -= 8;
                }
                //将剩余没有解析的字节数组 留下 剔除已经解析的字节数组，留待下一条消息来拼接解析
                Array.Copy(cacheBytes, nowIndex, cacheBytes, 0, cacheNum - nowIndex);
                cacheNum = cacheNum - nowIndex;
                break;
            }
        }
    }
    //关闭连接
    public void Close()
    {
        if (socket != null)
        {
            print("客户端主动断开连接");

            //主动发送一条断开连接的消息给服务器
            //QuitMsg quitMsg = new QuitMsg();
            //socket.Send(quitMsg.WriteBytes());

            //socket.Shutdown(SocketShutdown.Both);
            //socket.Disconnect(false);
            //socket.Close();
            //socket = null;

            isConnected = false;
        }
    }
    private void OnDestroy()
    {
        Close();
    }
}
