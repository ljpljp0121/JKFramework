using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Socket_1 : MonoBehaviour
{
    void Start()
    {
        //创建套接字Socket
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //用connect方法与服务端相连
        //确定服务端的IP和端口
        IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.01"), 8080);
        try
        {
            socket.Connect(iPEndPoint);
        }
        catch (SocketException e)
        {
            if (e.ErrorCode == 10061)
            {
                print("服务器拒绝连接");
            }
            else
            {
                print("连接服务器失败" + e.ErrorCode);
            }
        }
        //用send和receive相关方法收发数据
        //接受
        byte[] receiveBytes = new byte[1024];
        int receiveNum = socket.Receive(receiveBytes);
        print("收到服务端发来的消息" + Encoding.UTF8.GetString(receiveBytes,0,receiveNum));
        //发送
        socket.Send(Encoding.UTF8.GetBytes("你好,我是客户端"));
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }

    void Update()
    {

    }
}
