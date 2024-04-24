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
    //socket�Ƿ�ر�
    private bool isClose = true;

    //������Ϣ�Ķ��� �ڶ��߳�������Բ���
    private Queue<BaseInfo> receiveQueue = new Queue<BaseInfo>();

    //������Ϣ������
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
    /// �����ͻ���socket��ط���
    /// </summary>
    /// <param name="ip">Զ�˷�����ip</param>
    /// <param name="port">Զ�˷������˿�</param>
    public void StartClient(string ip, int port)
    {
        //����״̬�����ٿ�
        if (!isClose)
        {
            return;
        }
        //�ȼ�¼��������ַ
        serverIpPoint = new IPEndPoint(IPAddress.Parse(ip), port);

        IPEndPoint clientIpPort = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8081);
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(clientIpPort);
            isClose = false;
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            //����һ�������������ڴ�Ž�����Ϣ
            args.SetBuffer(cacheBytes, 0, cacheBytes.Length);
            //������Ϣ��Ὣ���ͷ���ip�Ͷ˿ڼ�������
            args.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            args.Completed += ReceiveMsg;
            socket.ReceiveFromAsync(args);
            print("�ͻ�����������");
        }
        catch (Exception e)
        {
            print("����socket������" + e.Message);
        }
    }
    //������Ϣ
    private void ReceiveMsg(object obj, SocketAsyncEventArgs args)
    {
        int nowIndex;
        int msgID;
        int msgLength;
        if (args.SocketError == SocketError.Success)
        {
            try
            {
                //����Ƿ������ĵ�ַ�Ļ��Ŵ���
                if (args.RemoteEndPoint.Equals(serverIpPoint))
                {
                    //�����������������Ϣ
                    nowIndex = 0;
                    //����ID
                    msgID = BitConverter.ToInt32(cacheBytes, nowIndex);
                    nowIndex += 4;
                    //��������
                    msgLength = BitConverter.ToInt32(cacheBytes, nowIndex);
                    nowIndex += 4;
                    //������Ϣ��
                    BaseInfo info = null;
                    switch (msgID)
                    {
                        case 1001:
                            info = new PlayerInfo();
                            //�����л���Ϣ��
                            info.ReadBytes(cacheBytes, nowIndex);
                            break;
                    }
                    if (info != null)
                    {
                        receiveQueue.Enqueue(info);
                    }
                }
                //�ٴν�����Ϣ
                if (socket != null && !isClose)
                {
                    args.SetBuffer(0, cacheBytes.Length);
                    socket.ReceiveFromAsync(args);
                }
            }
            catch (SocketException e)
            {
                print("������Ϣ����" + e.SocketErrorCode + e.Message);
            }
            catch (Exception ex)
            {
                print("������Ϣ����(����������)" + ex.Message);
            }
        }
        else
        {
            print("������Ϣʧ��" + args.SocketError);
        }

    }
    //������Ϣ
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
                //����Զ��Ŀ��
                args.RemoteEndPoint = serverIpPoint;
                socket.SendToAsync(args);
            }
        }
        catch (SocketException e)
        {
            print("������Ϣ����" + e.SocketErrorCode + e.Message);
        }
        catch (Exception ex)
        {
            print("������Ϣ����(����ʱ���л�����)" + ex.Message);
        }
    }

    private void SendToCallBack(object sender, SocketAsyncEventArgs e)
    {
        if (e.SocketError != SocketError.Success)
        {
            print("������Ϣʧ��" + e.SocketError);
        }
    }

    //�ر�socket
    public void Close()
    {
        if (socket != null)
        {
            isClose = true;
            //�����˳���Ϣ��������
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
