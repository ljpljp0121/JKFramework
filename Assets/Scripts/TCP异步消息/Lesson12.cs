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
        //    print("倒计时结束");
        //});
        //print("异步执行后的逻辑");
        CountDownAsync(7);
        print("异步执行后的逻辑");

        Socket socketTcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //服务器相关
        socketTcp.BeginAccept(AcceptCallBack, socketTcp);
    }

    void Update()
    {

    }

    public async void CountDownAsync(int second)
    {
        print("倒计时开始");

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

        print("倒计时结束");
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
        print("开始倒计时");
    }

    private void AcceptCallBack(IAsyncResult result)
    {
        Socket socketTcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //服务器相关
        socketTcp.BeginAccept((result) =>
        {
            try
            {//获取传入的参数
                Socket s = result.AsyncState as Socket;
                //通过调用EndAccept 得到连接的客户端Socket;
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
