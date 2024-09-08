using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAStar : MonoBehaviour
{
    //���Ͻ���ʼλ��
    public int beginX = -3;
    public int beginY = 5;
    //ÿ������֮��ƫ��λ��
    public int offsetX = 2;
    public int offsetY = 2;
    public int mapW = 20;
    public int mapH = 20;

    //��ʼ�����һ��Ϊ���������
    private Vector2 beginPos = Vector2.right * -1;
    private Dictionary<string, GameObject> cubes = new Dictionary<string, GameObject>();
    List<AStarNode> list;


    public Material red;
    public Material yellow;
    public Material blue;
    public Material white;

    void Start()
    {
        AStarMgr.Instance.InitMapInfo(mapW, mapH);
        for (int i = 0; i < mapW; i++)
        {
            for (int j = 0; j < mapH; j++)
            {
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.transform.position = new Vector3(beginX + i * offsetX, beginY + j * offsetY, 0);
                //����
                obj.name = i + "_" + j;
                //�洢�����嵽�ֵ�������
                cubes.Add(obj.name, obj);

                AStarNode node = AStarMgr.Instance.nodes[i, j];
                if (node.type == E_Node_Type.Stop)
                {
                    obj.GetComponent<MeshRenderer>().material = red;
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //���߼��
            RaycastHit info;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out info, 1000))
            {
                if (beginPos == Vector2.right * -1)
                {
                    //������һ��·��
                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            cubes[list[i].x + "_" + list[i].y].GetComponent<MeshRenderer>().material = white;
                        }
                    }

                    string[] strs = info.collider.gameObject.name.Split('_');
                    beginPos = new Vector2(int.Parse(strs[0]), int.Parse(strs[1]));
                    info.collider.gameObject.GetComponent<MeshRenderer>().material = yellow;
                }
                //������ˣ�����յ㿪ʼѰ·
                else
                {
                    string[] strs = info.collider.gameObject.name.Split('_');
                    Vector2 endPos = new Vector2(int.Parse(strs[0]), int.Parse(strs[1]));

                    list = AStarMgr.Instance.FindPath(beginPos,endPos);
                    cubes[(int)beginPos.x + "_" + (int)beginPos.y].GetComponent<MeshRenderer>().material = white;
                    if(list != null)
                    {
                        for(int i = 0; i< list.Count; i++)
                        {
                            cubes[list[i].x + "_" + list[i].y].GetComponent<MeshRenderer>().material = blue;
                        }
                    }

                    beginPos = Vector2.right * -1;

                }


            }
        }
    }
}
