using JKFrame;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class NetAsyncMgr : MonoBehaviour
{
    private static NetAsyncMgr instance;
    public static NetAsyncMgr Instance => instance;
    //和服务器连接的socket
    private Socket socket;

    //接受消息缓存容器
    private byte[] cacheBytes = new byte[1024 * 1024];
    private int cacheNum = 0;

    private Queue<BaseInfo> receiveQueue = new Queue<BaseInfo>();

    private int SEND_HEART_MSG_TIME = 2;
    private Heart heart = new Heart();

    void Awake()
    {
        instance = this;
        //过场景不移除
        DontDestroyOnLoad(gameObject);
        InvokeRepeating("SendHeartMsg", 0, SEND_HEART_MSG_TIME);
    }
    //发送心跳消息
    private void SendHeartMsg()
    {
        if (socket != null && socket.Connected)
            Send(heart);
    }

    void Update()
    {

    }

    //连接服务器
    public void Connect(string ip, int port)
    {
        if (socket != null && socket.Connected)
            return;

        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        SocketAsyncEventArgs acceptArgs = new SocketAsyncEventArgs();
        acceptArgs.RemoteEndPoint = ipPoint;
        acceptArgs.Completed += (socket, args) =>
        {
            if (args.SocketError == SocketError.Success)
            {
                print("连接成功");
                //收消息
                SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
                receiveArgs.SetBuffer(cacheBytes, 0, cacheBytes.Length);
                receiveArgs.Completed += ReceiveCallBack;
                this.socket.ReceiveAsync(receiveArgs);
            }
            else
            {
                print("连接失败" + args.SocketError);
            }
        };
        socket.ConnectAsync(acceptArgs);
    }

    private void ReceiveCallBack(object obj, SocketAsyncEventArgs args)
    {
        if (args.SocketError == SocketError.Success)
        {
            //解析消息 
            HandleReceiveMsg(args.BytesTransferred);
            //继续接受消息
            args.SetBuffer(0, args.Buffer.Length);
            //继续异步接受消息
            if (this.socket != null && this.socket.Connected)
            {
                socket.ReceiveAsync(args);
            }
            else
            {
                Close();
            }
        }
        else
        {
            print("接受消息失败" + args.SocketError);
            //关闭客户端连接
            Close();
        }
    }

    public void Send(BaseInfo msg)
    {
        if (this.socket != null && this.socket.Connected)
        {
            byte[] bytes = msg.WriteBytes();
            SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
            sendArgs.SetBuffer(bytes, 0, bytes.Length);
            sendArgs.Completed += (socket, args) =>
            {
                if (args.SocketError != SocketError.Success)
                {
                    print("发送消息失败" + args.SocketError);
                    Close();
                }
            };
            this.socket.SendAsync(sendArgs);
        }
        else
        {
            Close();
        }
    }

    public void Close()
    {
        if (socket != null)
        {
            QuitMsg msg = new QuitMsg();
            socket.Send(msg.WriteBytes());
            socket.Shutdown(SocketShutdown.Both);
            socket.Disconnect(false);
            socket.Close();
            socket = null;
        }
    }


    private void HandleReceiveMsg(int receiveNum)
    {
        int msgID = 0;
        int msgLength = 0;
        int nowIndex = 0;
        //由于消息接收后是直接存储在 cacheBytes中的 所以不需要进行什么拷贝操作
        //收到消息的字节数量
        cacheNum += receiveNum;
        while (true)
        {
            //每次将长度设置为-1 是避免上一次解析的数据 影响这一次的判断
            msgLength = -1;
            if (cacheNum - nowIndex >= 8)
            {
                //解析ID
                msgID = BitConverter.ToInt32(cacheBytes, nowIndex);
                nowIndex += 4;
                //解析长度
                msgLength = BitConverter.ToInt32(cacheBytes, nowIndex);
                nowIndex += 4;
            }

            if (cacheNum - nowIndex >= msgLength && msgLength != -1)
            {
                //解析消息体
                BaseInfo baseMsg = null;
                switch (msgID)
                {
                    case 1001:
                        baseMsg = new PlayerInfo();
                        baseMsg.ReadBytes(cacheBytes, nowIndex);
                        break;
                    case 1003:
                        baseMsg = new QuitMsg();
                        //由于该消息没有消息体 所以都不用反序列化
                        break;
                    case 999:
                        baseMsg = new Heart();
                        //由于该消息没有消息体 所以都不用反序列化
                        break;
                }
                if (baseMsg != null)
                {
                    receiveQueue.Enqueue(baseMsg);
                }
                nowIndex += msgLength;
                if (nowIndex == cacheNum)
                {
                    cacheNum = 0;
                    break;
                }
            }
            else
            {
                //如果不满足 证明有分包 
                //那么我们需要把当前收到的内容 记录下来
                //有待下次接受到消息后 再做处理
                //receiveBytes.CopyTo(cacheBytes, 0);
                //cacheNum = receiveNum;
                //如果进行了 id和长度的解析 但是 没有成功解析消息体 那么我们需要减去nowIndex移动的位置
                if (msgLength != -1)
                    nowIndex -= 8;
                //就是把剩余没有解析的字节数组内容 移到前面来 用于缓存下次继续解析
                Array.Copy(cacheBytes, nowIndex, cacheBytes, 0, cacheNum - nowIndex);
                cacheNum = cacheNum - nowIndex;
                break;
            }
        }
    }
    private void OnDestroy()
    {
        Close();
    }
}
