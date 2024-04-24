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
        //�����׽���Socket
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //��connect��������������
        //ȷ������˵�IP�Ͷ˿�
        IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.01"), 8080);
        try
        {
            socket.Connect(iPEndPoint);
        }
        catch (SocketException e)
        {
            if (e.ErrorCode == 10061)
            {
                print("�������ܾ�����");
            }
            else
            {
                print("���ӷ�����ʧ��" + e.ErrorCode);
            }
        }
        //��send��receive��ط����շ�����
        //����
        byte[] receiveBytes = new byte[1024];
        int receiveNum = socket.Receive(receiveBytes);
        print("�յ�����˷�������Ϣ" + Encoding.UTF8.GetString(receiveBytes,0,receiveNum));
        //����
        socket.Send(Encoding.UTF8.GetBytes("���,���ǿͻ���"));
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }

    void Update()
    {

    }
}
