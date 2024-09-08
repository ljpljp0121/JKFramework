using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 格子类型
/// </summary>
public enum E_Node_Type
{
    Walk,//可行走
    Stop,//无法行走
}

/// <summary>
/// A星寻路格子类
/// </summary>
public class AStarNode
{
    //格子对象坐标
    public int x;
    public int y;

    public float f; // 寻路消耗
    public float g; // 离起点距离
    public float h; // 离终点距离
    public AStarNode father; //父对象

    public E_Node_Type type; // 格子类型

    public AStarNode(int x, int y, E_Node_Type type)
    {
        this.x = x; this.y = y;
        this.type = type;
    }
}
