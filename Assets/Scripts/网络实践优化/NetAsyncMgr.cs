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
    //�ͷ��������ӵ�socket
    private Socket socket;

    //������Ϣ��������
    private byte[] cacheBytes = new byte[1024 * 1024];
    private int cacheNum = 0;

    private Queue<BaseInfo> receiveQueue = new Queue<BaseInfo>();

    private int SEND_HEART_MSG_TIME = 2;
    private Heart heart = new Heart();

    void Awake()
    {
        instance = this;
        //���������Ƴ�
        DontDestroyOnLoad(gameObject);
        InvokeRepeating("SendHeartMsg", 0, SEND_HEART_MSG_TIME);
    }
    //����������Ϣ
    private void SendHeartMsg()
    {
        if (socket != null && socket.Connected)
            Send(heart);
    }

    void Update()
    {

    }

    //���ӷ�����
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
                print("���ӳɹ�");
                //����Ϣ
                SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
                receiveArgs.SetBuffer(cacheBytes, 0, cacheBytes.Length);
                receiveArgs.Completed += ReceiveCallBack;
                this.socket.ReceiveAsync(receiveArgs);
            }
            else
            {
                print("����ʧ��" + args.SocketError);
            }
        };
        socket.ConnectAsync(acceptArgs);
    }

    private void ReceiveCallBack(object obj, SocketAsyncEventArgs args)
    {
        if (args.SocketError == SocketError.Success)
        {
            //������Ϣ 
            HandleReceiveMsg(args.BytesTransferred);
            //����������Ϣ
            args.SetBuffer(0, args.Buffer.Length);
            //�����첽������Ϣ
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
            print("������Ϣʧ��" + args.SocketError);
            //�رտͻ�������
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
                    print("������Ϣʧ��" + args.SocketError);
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
        //������Ϣ���պ���ֱ�Ӵ洢�� cacheBytes�е� ���Բ���Ҫ����ʲô��������
        //�յ���Ϣ���ֽ�����
        cacheNum += receiveNum;
        while (true)
        {
            //ÿ�ν���������Ϊ-1 �Ǳ�����һ�ν��������� Ӱ����һ�ε��ж�
            msgLength = -1;
            if (cacheNum - nowIndex >= 8)
            {
                //����ID
                msgID = BitConverter.ToInt32(cacheBytes, nowIndex);
                nowIndex += 4;
                //��������
                msgLength = BitConverter.ToInt32(cacheBytes, nowIndex);
                nowIndex += 4;
            }

            if (cacheNum - nowIndex >= msgLength && msgLength != -1)
            {
                //������Ϣ��
                BaseInfo baseMsg = null;
                switch (msgID)
                {
                    case 1001:
                        baseMsg = new PlayerInfo();
                        baseMsg.ReadBytes(cacheBytes, nowIndex);
                        break;
                    case 1003:
                        baseMsg = new QuitMsg();
                        //���ڸ���Ϣû����Ϣ�� ���Զ����÷����л�
                        break;
                    case 999:
                        baseMsg = new Heart();
                        //���ڸ���Ϣû����Ϣ�� ���Զ����÷����л�
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
                //��������� ֤���зְ� 
                //��ô������Ҫ�ѵ�ǰ�յ������� ��¼����
                //�д��´ν��ܵ���Ϣ�� ��������
                //receiveBytes.CopyTo(cacheBytes, 0);
                //cacheNum = receiveNum;
                //��������� id�ͳ��ȵĽ��� ���� û�гɹ�������Ϣ�� ��ô������Ҫ��ȥnowIndex�ƶ���λ��
                if (msgLength != -1)
                    nowIndex -= 8;
                //���ǰ�ʣ��û�н������ֽ��������� �Ƶ�ǰ���� ���ڻ����´μ�������
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
