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

    //�ͻ���Socket
    private Socket socket;
    //���ڷ�����Ϣ�Ķ��У����߳�������� �����߳�ȡ
    private Queue<BaseInfo> sendMsgQueue = new Queue<BaseInfo>();
    //���ڽ�����Ϣ�Ķ��� ���߳������ ���߳���ȡ
    private Queue<BaseInfo> receiveQueue = new Queue<BaseInfo>();


    ////��������Ϣ������
    //private byte[] receiveBytes = new byte[1024 * 1024];
    ////�����յ����ֽ���
    //private int receiveNum;

    //������ֽ�����ͳ��� ����ְ�
    private byte[] cacheBytes = new byte[1024 * 1024];
    private int cacheNum = 0;

    //�Ƿ�����
    private bool isConnected = false;

    private int SEND_HEART_MSG_TIME = 2;
    private Heart heart = new Heart();

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

        //��ʱѭ������������Ϣ
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

    //���ӷ����
    public void Connect(string ip, int port)
    {
        //���������״̬ ֱ�ӷ���
        if (isConnected)
        {
            return;
        }
        if (socket == null)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        //���ӷ����
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        try
        {
            socket.Connect(ipPoint);
            isConnected = true;
            //���������߳�
            ThreadPool.QueueUserWorkItem(SendMsg);
            //sendThread = new Thread(SendMsg);
            //sendThread.Start();
            //���������߳�
            ThreadPool.QueueUserWorkItem(ReceiveMsg);
            //receiveThread = new Thread(ReceiveMsg);
            //receiveThread.Start();
        }
        catch (SocketException e)
        {
            if (e.ErrorCode == 10061)
                print("�������ܾ�����");
            print("����ʧ��" + e.ErrorCode + e.Message);
        }
    }
    //������Ϣ
    public void Send(BaseInfo info)
    {
        sendMsgQueue.Enqueue(info);
    }
    /// <summary>
    /// ���ڲ��� ֱ�ӷ��ֽ�����
    /// </summary>
    /// <param name="bytes"></param>
    public void SendTest(byte[] bytes)
    {
        socket.Send(bytes);
    }

    //���߳� ����Ϣ���з�����Ϣ
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
    //���Ͻ�����Ϣ
    private void ReceiveMsg(object obj)
    {
        while (isConnected)
        {
            if (socket.Available > 0)
            {
                byte[] receiveBytes = new byte[1024 * 1024];
                int receiveNum = socket.Receive(receiveBytes);
                HandleReceiveMsg(receiveBytes, receiveNum);
                ////���Ƚ�����ϢID
                ////ʹ���ֽ�����ǰ�ĸ��ֽڵõ�ID
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
                ////�����ϢΪ�� û�н���
                //if (baseInfo == null)
                //    continue;
                ////�յ���Ϣ ����Ϊ�ַ��� ��������
                //receiveQueue.Enqueue(baseInfo);
            }
        }
    }
    //������Ϣ ����ְ� ճ��
    private void HandleReceiveMsg(byte[] receiveBytes, int receiveNum)
    {
        int msgID = 0;
        int msgLength = 0;
        int nowIndex = 0;

        //�յ���Ϣ�ǲ鿴 �����Ƿ�Ϊ�� ����ƴ�ӵ�����
        receiveBytes.CopyTo(cacheBytes, cacheNum);
        cacheNum += receiveNum;

        while (true)
        {
            //ÿ�ν���������Ϊ-1 ������һ�ν�������Ӱ����һ���ж�
            msgLength = -1;
            //�������һ����Ϣ
            if (cacheNum - nowIndex >= 8)
            {
                //����ID
                msgID = BitConverter.ToInt32(cacheBytes, nowIndex);
                nowIndex += 4;
                //��������
                msgLength = BitConverter.ToInt32(cacheBytes, nowIndex);
                nowIndex += 4;
            }
            //���ȴ���Length�Ž���
            if (cacheNum - nowIndex >= msgLength && msgLength != -1)
            {
                //������Ϣ��
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
                //���ǲ�����˵���зְ���
                //Ҫ��������������һ�ν��յ���Ϣ����ƴ������ ��������
                //receiveBytes.CopyTo(cacheBytes, 0);
                //cacheNum = receiveNum;
                //���������id�ͳ��Ƚ��� ����û�гɹ�������Ϣ�� ��ô��Ҫ��ȥnowIndex�ƶ�λ��
                if (msgLength != -1)
                {
                    nowIndex -= 8;
                }
                //��ʣ��û�н������ֽ����� ���� �޳��Ѿ��������ֽ����飬������һ����Ϣ��ƴ�ӽ���
                Array.Copy(cacheBytes, nowIndex, cacheBytes, 0, cacheNum - nowIndex);
                cacheNum = cacheNum - nowIndex;
                break;
            }
        }
    }
    //�ر�����
    public void Close()
    {
        if (socket != null)
        {
            print("�ͻ��������Ͽ�����");

            //��������һ���Ͽ����ӵ���Ϣ��������
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
