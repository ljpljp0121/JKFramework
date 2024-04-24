using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Lesson14 : MonoBehaviour
{
    void Start()
    {
        //创建套接字
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //绑定本机地址
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
        socket.Bind(ipPoint);
        //发送到指定目标
        IPEndPoint remoteIpPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8081);
        //指定要发送的字节数 和远程计算机的IP 和 端口
        socket.SendTo(Encoding.UTF8.GetBytes("廖建鹏"), remoteIpPoint);
        //接受消息
        byte[] bytes = new byte[512];
        //初始化不重要 因为之后使用里面的值会被覆盖
        EndPoint remoteIpPoint2 = new IPEndPoint(IPAddress.Any, 0);
        int length = socket.ReceiveFrom(bytes, ref remoteIpPoint2);
        print((remoteIpPoint2 as  IPEndPoint).Address.ToString() + "发来了" +
            Encoding.UTF8.GetString(bytes,0,length));
        //释放关闭
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }

    void Update()
    {

    }
}
