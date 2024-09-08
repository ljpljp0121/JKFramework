using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Lesson1 :EditorWindow
{
    [MenuItem("Unity编辑器拓展/Lesson2/显示自定义面板")]
    private static void ShowWindow()
    {
       Lesson1 win =  EditorWindow.GetWindow<Lesson1>();
        win.Show();
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnGUI()
    {
        GUILayout.Label("测试文本");
        if (GUILayout.Button("测试按钮"))
        {
            Debug.Log("Test");
        }

        if (Event.current.type == EventType.ExecuteCommand)
        {
            if (Event.current.commandName == "廖建鹏的事件")
            {
                Debug.Log("收到廖建鹏的事件");
            }
        }
    }

}
