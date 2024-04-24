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
    //socket�Ƿ�ر�
    private bool isClose = true;

    //�������� ���� 
    //���� �ͷ�����Ϣ�Ķ��� �ڶ��߳�������Բ���
    private Queue<BaseInfo> sendQueue = new Queue<BaseInfo>();
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
            print("�ͻ�����������");
            ThreadPool.QueueUserWorkItem(ReceiveMsg);
            ThreadPool.QueueUserWorkItem(SendMsg);
        }
        catch (Exception e)
        {
            print("����socket������" + e.Message);
        }
    }
    //������Ϣ
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
                    //Ϊ�˱��⴦�� �Ƿ�����������ɧ����Ϣ
                    if (!tempIpPoint.Equals(serverIpPoint))
                    {
                        continue;
                    }
                    else
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
        }
    }
    //������Ϣ�߳�
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
                    print("������Ϣ����" + e.SocketErrorCode + e.Message);
                }
            }
        }
    }
    //���ⲿ���õķ�����Ϣ���� ����Ϣѹ�����������
    public void Send(BaseInfo info)
    {
        sendQueue.Enqueue(info);
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
