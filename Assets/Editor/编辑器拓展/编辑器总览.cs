using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum E_TestType
{
    one = 1, two = 2, three = 4, four, five, One_and_Two = 1 | 2
}

public class 编辑器总览 : EditorWindow
{
    [MenuItem("Unity编辑器拓展/Lesson3/EditorGUI和EditorGUIUtility学习")]
    private static void OpenLesson3()
    {
        编辑器总览 win = EditorWindow.GetWindow<编辑器总览>("LJP");
        //win.titleContent = new GUIContent("EditorGUI");
        win.Show();
    }
    #region 编辑器参数
    //Lesson4
    int layer;
    string tag;
    Color color;
    //Lesson5
    E_TestType type;
    E_TestType type2;
    int index;
    int[] num = { 123, 234, 345 };
    string[] strs = { "123", "234", "345", "哈哈哈" };
    //Lesson6
    AudioClip obj;
    int i;
    //Lesson7
    bool isHide;
    bool isHideGroup;
    //Lesson8
    bool isTogHide;
    bool isTog;
    bool isTogLeft;
    bool isTogGroup;
    //Lesson9
    bool isSliderHide;
    float fSlider;
    int iSlider;
    float leftV;
    float rightV;
    //Lesson10
    bool isHelpHide;
    //Lesson11
    bool isCurveHide;
    AnimationCurve curve = new AnimationCurve();
    Vector2 vec2Pos;
    //Lesson13
    private Texture img;
    bool isLoadHide;

    private Texture img3;
    bool isFindHide;

    bool isSendHide;

    bool isCursorHide;

    bool isColorHide;
    #endregion
    private void OnGUI()
    {
        vec2Pos = EditorGUILayout.BeginScrollView(vec2Pos);
        #region EditorGUI

        #region Lesson4 文本控件、层级标签、颜色获取控件
        isTogGroup = EditorGUILayout.BeginToggleGroup("文本控件、层级标签、颜色获取控件", isTogGroup);
        if (isTogGroup)
        {
            EditorGUILayout.LabelField("文本标签", "测试内容");
            layer = EditorGUILayout.LayerField("层级选择", layer);
            tag = EditorGUILayout.TagField("标签选择", tag);
            color = EditorGUILayout.ColorField(new GUIContent("自定义颜色获取"), color);
        }
        EditorGUILayout.EndToggleGroup();
        #endregion

        #region Lesson5 枚举选择、整数选择、按下触发按钮
        isHide = EditorGUILayout.Foldout(isHide, "枚举选择、整数选择、按下触发按钮", true);
        if (isHide)
        {
            type = (E_TestType)EditorGUILayout.EnumPopup("枚举选择", type);
            type2 = (E_TestType)EditorGUILayout.EnumFlagsField("枚举多选", type2);
            index = EditorGUILayout.IntPopup("整数单选框", index, strs, num);
            EditorGUILayout.LabelField(index.ToString());
            if (EditorGUILayout.DropdownButton(new GUIContent("按钮上文字"), FocusType.Passive))
            {
                Debug.Log("按下按钮响应");
            }
        }
        #endregion

        #region Lesson6 对象关联和各类型输入控件
        isHideGroup = EditorGUILayout.BeginFoldoutHeaderGroup(isHideGroup, "对象关联和各类型输入控件");
        if (isHideGroup)
        {
            obj = (AudioClip)EditorGUILayout.ObjectField("关联资源对象", obj, typeof(AudioClip), false);
            i = EditorGUILayout.IntField("Int输入框", i);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        //long变量 = EditorGUILayout.LongField("long输入框", long变量);
        //float变量 = EditorGUILayout.FloatField("Float 输入：", float变量);
        //double变量 = EditorGUILayout.DoubleField("double 输入：", double变量);

        //string变量 = EditorGUILayout.TextField("Text输入：", string变量);
        //vector2变量 = EditorGUILayout.Vector2Field("Vec2输入： ", vector2变量);
        //vector3变量 = EditorGUILayout.Vector3Field("Vec3输入： ", vector3变量);
        //vector4变量 = EditorGUILayout.Vector4Field("Vec4输入： ", vector4变量);
        //rect变量 = EditorGUILayout.RectField("rect输入： ", rect变量);
        //bounds变量 = EditorGUILayout.BoundsField("Bounds输入： ", bounds变量);
        //boundsInt变量 = EditorGUILayout.BoundsIntField("Bounds输入： ", boundsInt变量);
        #endregion

        #region Lesson7 折叠、折叠组控件控件
        //isHide = EditorGUILayout.Foldout(isHide,"折叠控件",true);

        //要成对出现Begin和End;
        //isHideGroup = EditorGUILayout.BeginFoldoutHeaderGroup(isHideGroup, "折叠组控件");

        //EditorGUILayout.EndFoldoutHeaderGroup();
        #endregion

        #region Lesson8 开关、开关组控件
        isTogHide = EditorGUILayout.Foldout(isTogHide, "开关、开关组控件", true);
        if (isTogHide)
        {
            isTog = EditorGUILayout.Toggle("开关控件", isTog);
            isTogLeft = EditorGUILayout.ToggleLeft("左侧开关", isTogLeft);
            isTogGroup = EditorGUILayout.BeginToggleGroup("开关组控件", isTogGroup);
            EditorGUILayout.EndToggleGroup();
        }
        #endregion

        #region Lesson9 滑动条，双块滑动条控件
        isSliderHide = EditorGUILayout.Foldout(isSliderHide, "滑动条，双块滑动条控件", true);
        if (isSliderHide)
        {
            fSlider = EditorGUILayout.Slider("滑动条", fSlider, 0.0f, 10f);
            iSlider = EditorGUILayout.IntSlider("整型滑动条", iSlider, 0, 10);
            EditorGUILayout.MinMaxSlider("双块滑动条", ref leftV, ref rightV, 0, 10);
            EditorGUILayout.LabelField(leftV.ToString());
            EditorGUILayout.LabelField(rightV.ToString());
        }
        #endregion

        #region Lesson10 帮助框、间隔控件
        isHelpHide = EditorGUILayout.Foldout(isHelpHide, "帮助框、间隔控件", true);
        if (isHelpHide)
        {
            EditorGUILayout.HelpBox("一般提示", MessageType.None);
            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("感叹号提示", MessageType.Info);
            EditorGUILayout.HelpBox("警告提示", MessageType.Warning);
            EditorGUILayout.HelpBox("错误符号提示", MessageType.Error);
        }
        #endregion

        #region Lesson11 动画曲线控件、布局API
        isCurveHide = EditorGUILayout.Foldout(isCurveHide, "动画曲线控件、布局API", true);
        if (isCurveHide)
        {
            curve = EditorGUILayout.CurveField("曲线控件", curve);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("123123");
            EditorGUILayout.LabelField("123123");
            EditorGUILayout.LabelField("123123");
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #endregion

        #region EditorGuiUtility公共类
        #region Lesson13 资源加载
        #region 知识点一 Editor Default Resources文件夹
        //Editor Default Resources 也是Unity当中的一个特殊文件夹
        //它的主要作用是放置提供给 EditorGUIUtility 加载的资源
        //想要使用EditorGUIUtility公共类来加载资源
        //我们需要将资源放置在 Editor Default Resources 文件夹中
        #endregion
        #region 知识点二 加载资源（如果资源不存在返回null)
        //对应API：
        //EditorGUIUtility.Load
        //注意事项：
        //1.只能加载Assets/Editor Default Resources/文件夹下的资源
        //2.加载资源时，需要填写资源后缀名
        #endregion
        #region 知识点三 加载资源（如果资源不存在会直接报错）
        //对应API：
        //EditorGUIUtility.LoadRequired
        //注意事项：
        //1.只能加载Assets/Editor Default Resources/文件夹下的资源
        //2.加载资源时，需要填写资源后缀名
        #endregion
        isLoadHide = EditorGUILayout.Foldout(isLoadHide, "资源加载类型", true);
        if (isLoadHide)
        {
            if (GUILayout.Button("加载编辑器图片资源"))
            {
                img = EditorGUIUtility.Load("EditorTeach.png") as Texture;
            }
            if (img != null)
            {
                GUI.DrawTexture(new Rect(0, 300, 160, 90), img);
            }
        }
        #endregion

        #region Lesson 14 搜索栏查询,对象选中提示
        isFindHide = EditorGUILayout.Foldout(isFindHide, "搜索栏查询,对象选中提示", true);
        if (isFindHide)
        {
            if (GUILayout.Button("打开搜索框查询窗口"))
            {
                EditorGUIUtility.ShowObjectPicker<Texture>(null, true, "", 0);
            }

            if (Event.current.commandName == "ObjectSelectorUpdated")
            {
                img3 = EditorGUIUtility.GetObjectPickerObject() as Texture;
                if (img3 != null)
                {
                    Debug.Log(img3.name);
                }
            }

            if (GUILayout.Button("高亮选中对象"))
            {
                if (img3 != null)
                    EditorGUIUtility.PingObject(img3);
            }
        }
        #endregion

        #region Lesson 15 窗口事件传递，坐标转换
        isSendHide = EditorGUILayout.Foldout(isSendHide, "窗口事件传递，坐标转换", true);
        if (isSendHide)
        {
            if (GUILayout.Button("传递事件"))
            {
                Event e = EditorGUIUtility.CommandEvent("廖建鹏的事件");
                Lesson1 win = EditorWindow.GetWindow<Lesson1>();
                win.SendEvent(e);
                //传递事件时，会自动将接受的窗口打开
            }


        }

        #endregion

        #region Lesson16 指定区域使用对应鼠标指针
        isCursorHide = EditorGUILayout.Foldout(isCursorHide, "指定区域使用对应指针", true);
        if (isCursorHide)
        {
            EditorGUI.DrawRect(new Rect(0, 250, 100, 100), Color.yellow);
            EditorGUIUtility.AddCursorRect(new Rect(0,250,100,100),MouseCursor.Text);
        }

        #endregion

        #region Lesson17 绘制色板，绘制曲线
        isColorHide = EditorGUILayout.Foldout(isColorHide, "绘制色板，绘制曲线", true);
        if (isColorHide)
        {
            EditorGUIUtility.DrawColorSwatch(new Rect(0, 250, 100, 100), color);
            EditorGUIUtility.DrawCurveSwatch(new Rect(100,250,100,100),curve,null,Color.red,Color.white);
        }
        #endregion
        #endregion

        #region Selection公共类

        #endregion

        EditorGUILayout.EndScrollView();
    }


    void Start()
    {
        #region 知识点一 EditorGUIUtility公共类是用来做什么的？
        // Utility是实用的意思，EditorGUIUtility 是 EditorGUI 中的一个实用工具类
        // 提供了一些 EditorGUI 相关的其他辅助API
        // 我们只需要学习其中的相对常用的内容

        // 官方文档：https://docs.unity3d.com/ScriptReference/EditorGUIUtility.html
        #endregion

        #region 知识点二 准备工作
        //创建一个自定义编辑器窗口 用于之后学习EditorGUIUtility相关的知识
        #endregion
    }
}
