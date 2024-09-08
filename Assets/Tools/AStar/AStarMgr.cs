using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A星寻路管理器
/// </summary>
public class AStarMgr
{
    private static AStarMgr instance;
    public static AStarMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AStarMgr();
            }
            return instance;
        }
    }

    //地图宽高
    public int mapW;
    public int mapH;

    public AStarNode[,] nodes; // 地图相关的格子对象容器

    //开启关闭列表
    private List<AStarNode> openList = new List<AStarNode>();
    private List<AStarNode> closeList = new List<AStarNode>();

    /// <summary>
    /// 初始化地图信息
    /// </summary>
    /// <param name="w"></param>
    /// <param name="h"></param>
    public void InitMapInfo(int w, int h)
    {
        mapH = h;
        mapW = w;

        nodes = new AStarNode[w, h];

        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                //随机生成阻挡
                AStarNode node = new AStarNode(i, j, Random.Range(0, 100) < 20 ? E_Node_Type.Stop : E_Node_Type.Walk);
                nodes[i, j] = node;
            }
        }
    }

    /// <summary>
    /// 寻路方法
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <returns></returns>
    public List<AStarNode> FindPath(Vector2 startPos, Vector2 endPos)
    {
        //开始或结束点在地图外
        if (startPos.x < 0 || startPos.x >= mapW || startPos.y < 0 || startPos.y >= mapH ||
            endPos.x < 0 || endPos.x >= mapW || endPos.y < 0 || endPos.y >= mapH)
        {
            return null;
        }
        AStarNode start = nodes[(int)startPos.x, (int)startPos.y];
        AStarNode end = nodes[(int)endPos.x, (int)endPos.y];
        //开始或结束点是阻挡格子
        if (start.type == E_Node_Type.Stop || end.type == E_Node_Type.Stop)
        {
            return null;
        }

        //清空关闭和开启列表
        closeList.Clear();
        openList.Clear();

        //把开始点放入关闭列表中
        start.father = null;
        start.f = 0;
        start.g = 0;
        start.h = 0;
        closeList.Add(start);
        while(true)
        {
            FindNearNodeToOpenList(start.x - 1, start.y - 1, 1.4f, start, end);
            FindNearNodeToOpenList(start.x, start.y - 1, 1f, start, end);
            FindNearNodeToOpenList(start.x + 1, start.y - 1, 1.4f, start, end);
            FindNearNodeToOpenList(start.x - 1, start.y, 1f, start, end);
            FindNearNodeToOpenList(start.x + 1, start.y, 1f, start, end);
            FindNearNodeToOpenList(start.x - 1, start.y + 1, 1.4f, start, end);
            FindNearNodeToOpenList(start.x, start.y + 1, 1f, start, end);
            FindNearNodeToOpenList(start.x + 1, start.y + 1, 1.4f, start, end);
            //死路判断
            if(openList.Count == 0)
            {
                Debug.Log("死路");
                return null;
            }
            openList.Sort(SortOpenList);
            closeList.Add(openList[0]);
            //找的这个点又变成新的起点
            start = openList[0];
            openList.RemoveAt(0);

            if (start == end)
            {
                List<AStarNode> path = new List<AStarNode>();
                path.Add(end);
                while(end.father != null)
                {
                    path.Add(end.father);
                    end = end.father;
                }
                path.Reverse();
                return path;
            }
        }
    }

    /// <summary>
    /// 排序函数
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private int SortOpenList(AStarNode a, AStarNode b)
    {
        if (a.f > b.f)
        {
            return 1;
        }
        else if(a.f < b.f)
        {
            return -1;
        }
        else
        {
            return 0; 
        }
    }

    /// <summary>
    /// 将邻近的点放入开启列表中
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void FindNearNodeToOpenList(int x, int y, float g, AStarNode father, AStarNode end)
    {
        if (x < 0 || x >= mapW || y < 0 || y >= mapH)
        {
            return;
        }
        AStarNode node = nodes[x, y];
        if (node == null || node.type == E_Node_Type.Stop || closeList.Contains(node) || openList.Contains(node))
        {
            return;
        }

        //计算f值
        //f = g + h
        node.father = father;
        node.g = father.g + g;
        node.h = Mathf.Abs(end.x - node.x) + Mathf.Abs(end.y - node.y);
        node.f = node.g + node.h;

        //通过验证后存到开启列表中
        openList.Add(node);
    }
}
