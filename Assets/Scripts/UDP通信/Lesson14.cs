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
        //�����׽���
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //�󶨱�����ַ
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
        socket.Bind(ipPoint);
        //���͵�ָ��Ŀ��
        IPEndPoint remoteIpPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8081);
        //ָ��Ҫ���͵��ֽ��� ��Զ�̼������IP �� �˿�
        socket.SendTo(Encoding.UTF8.GetBytes("�ν���"), remoteIpPoint);
        //������Ϣ
        byte[] bytes = new byte[512];
        //��ʼ������Ҫ ��Ϊ֮��ʹ�������ֵ�ᱻ����
        EndPoint remoteIpPoint2 = new IPEndPoint(IPAddress.Any, 0);
        int length = socket.ReceiveFrom(bytes, ref remoteIpPoint2);
        print((remoteIpPoint2 as  IPEndPoint).Address.ToString() + "������" +
            Encoding.UTF8.GetString(bytes,0,length));
        //�ͷŹر�
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }

    void Update()
    {

    }
}
