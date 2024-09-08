using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��������
/// </summary>
public enum E_Node_Type
{
    Walk,//������
    Stop,//�޷�����
}

/// <summary>
/// A��Ѱ·������
/// </summary>
public class AStarNode
{
    //���Ӷ�������
    public int x;
    public int y;

    public float f; // Ѱ·����
    public float g; // ��������
    public float h; // ���յ����
    public AStarNode father; //������

    public E_Node_Type type; // ��������

    public AStarNode(int x, int y, E_Node_Type type)
    {
        this.x = x; this.y = y;
        this.type = type;
    }
}
