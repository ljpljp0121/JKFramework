using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Lesson12 : MonoBehaviour
{
    void Start()
    {
        //CountDownAsync(5, () =>
        //{
        //    print("����ʱ����");
        //});
        //print("�첽ִ�к���߼�");
        CountDownAsync(7);
        print("�첽ִ�к���߼�");

        Socket socketTcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //���������
        socketTcp.BeginAccept(AcceptCallBack, socketTcp);
    }

    void Update()
    {

    }

    public async void CountDownAsync(int second)
    {
        print("����ʱ��ʼ");

        await Task.Run(() =>
        {
            while (true)
            {
                print(second);
                Thread.Sleep(1000);
                --second;
                if (second == 0)
                {
                    break;
                }
            }
        });

        print("����ʱ����");
    }

    public void CountDownAsync(int second, UnityAction callback)
    {
        Thread t = new Thread(() =>
        {
            while (true)
            {
                print(second);
                Thread.Sleep(1000);
                --second;
                if (second == 0)
                {
                    break;
                }
            }
            callback?.Invoke();
        });
        t.Start();
        print("��ʼ����ʱ");
    }

    private void AcceptCallBack(IAsyncResult result)
    {
        Socket socketTcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //���������
        socketTcp.BeginAccept((result) =>
        {
            try
            {//��ȡ����Ĳ���
                Socket s = result.AsyncState as Socket;
                //ͨ������EndAccept �õ����ӵĿͻ���Socket;
                Socket clientSocket = s.EndAccept(result);
                s.BeginAccept(AcceptCallBack, s);
            }
            catch (SocketException e)
            {
                print(e.ErrorCode);
            }
        }, socketTcp);
    }
}
