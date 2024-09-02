using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum E_TestType
{
    one = 1, two = 2, three = 4, four, five, One_and_Two = 1 | 2
}

public class �༭������ : EditorWindow
{
    [MenuItem("Unity�༭����չ/Lesson3/EditorGUI��EditorGUIUtilityѧϰ")]
    private static void OpenLesson3()
    {
        �༭������ win = EditorWindow.GetWindow<�༭������>("LJP");
        //win.titleContent = new GUIContent("EditorGUI");
        win.Show();
    }
    #region �༭������
    //Lesson4
    int layer;
    string tag;
    Color color;
    //Lesson5
    E_TestType type;
    E_TestType type2;
    int index;
    int[] num = { 123, 234, 345 };
    string[] strs = { "123", "234", "345", "������" };
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

        #region Lesson4 �ı��ؼ����㼶��ǩ����ɫ��ȡ�ؼ�
        isTogGroup = EditorGUILayout.BeginToggleGroup("�ı��ؼ����㼶��ǩ����ɫ��ȡ�ؼ�", isTogGroup);
        if (isTogGroup)
        {
            EditorGUILayout.LabelField("�ı���ǩ", "��������");
            layer = EditorGUILayout.LayerField("�㼶ѡ��", layer);
            tag = EditorGUILayout.TagField("��ǩѡ��", tag);
            color = EditorGUILayout.ColorField(new GUIContent("�Զ�����ɫ��ȡ"), color);
        }
        EditorGUILayout.EndToggleGroup();
        #endregion

        #region Lesson5 ö��ѡ������ѡ�񡢰��´�����ť
        isHide = EditorGUILayout.Foldout(isHide, "ö��ѡ������ѡ�񡢰��´�����ť", true);
        if (isHide)
        {
            type = (E_TestType)EditorGUILayout.EnumPopup("ö��ѡ��", type);
            type2 = (E_TestType)EditorGUILayout.EnumFlagsField("ö�ٶ�ѡ", type2);
            index = EditorGUILayout.IntPopup("������ѡ��", index, strs, num);
            EditorGUILayout.LabelField(index.ToString());
            if (EditorGUILayout.DropdownButton(new GUIContent("��ť������"), FocusType.Passive))
            {
                Debug.Log("���°�ť��Ӧ");
            }
        }
        #endregion

        #region Lesson6 ��������͸���������ؼ�
        isHideGroup = EditorGUILayout.BeginFoldoutHeaderGroup(isHideGroup, "��������͸���������ؼ�");
        if (isHideGroup)
        {
            obj = (AudioClip)EditorGUILayout.ObjectField("������Դ����", obj, typeof(AudioClip), false);
            i = EditorGUILayout.IntField("Int�����", i);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        //long���� = EditorGUILayout.LongField("long�����", long����);
        //float���� = EditorGUILayout.FloatField("Float ���룺", float����);
        //double���� = EditorGUILayout.DoubleField("double ���룺", double����);

        //string���� = EditorGUILayout.TextField("Text���룺", string����);
        //vector2���� = EditorGUILayout.Vector2Field("Vec2���룺 ", vector2����);
        //vector3���� = EditorGUILayout.Vector3Field("Vec3���룺 ", vector3����);
        //vector4���� = EditorGUILayout.Vector4Field("Vec4���룺 ", vector4����);
        //rect���� = EditorGUILayout.RectField("rect���룺 ", rect����);
        //bounds���� = EditorGUILayout.BoundsField("Bounds���룺 ", bounds����);
        //boundsInt���� = EditorGUILayout.BoundsIntField("Bounds���룺 ", boundsInt����);
        #endregion

        #region Lesson7 �۵����۵���ؼ��ؼ�
        //isHide = EditorGUILayout.Foldout(isHide,"�۵��ؼ�",true);

        //Ҫ�ɶԳ���Begin��End;
        //isHideGroup = EditorGUILayout.BeginFoldoutHeaderGroup(isHideGroup, "�۵���ؼ�");

        //EditorGUILayout.EndFoldoutHeaderGroup();
        #endregion

        #region Lesson8 ���ء�������ؼ�
        isTogHide = EditorGUILayout.Foldout(isTogHide, "���ء�������ؼ�", true);
        if (isTogHide)
        {
            isTog = EditorGUILayout.Toggle("���ؿؼ�", isTog);
            isTogLeft = EditorGUILayout.ToggleLeft("��࿪��", isTogLeft);
            isTogGroup = EditorGUILayout.BeginToggleGroup("������ؼ�", isTogGroup);
            EditorGUILayout.EndToggleGroup();
        }
        #endregion

        #region Lesson9 ��������˫�黬�����ؼ�
        isSliderHide = EditorGUILayout.Foldout(isSliderHide, "��������˫�黬�����ؼ�", true);
        if (isSliderHide)
        {
            fSlider = EditorGUILayout.Slider("������", fSlider, 0.0f, 10f);
            iSlider = EditorGUILayout.IntSlider("���ͻ�����", iSlider, 0, 10);
            EditorGUILayout.MinMaxSlider("˫�黬����", ref leftV, ref rightV, 0, 10);
            EditorGUILayout.LabelField(leftV.ToString());
            EditorGUILayout.LabelField(rightV.ToString());
        }
        #endregion

        #region Lesson10 �����򡢼���ؼ�
        isHelpHide = EditorGUILayout.Foldout(isHelpHide, "�����򡢼���ؼ�", true);
        if (isHelpHide)
        {
            EditorGUILayout.HelpBox("һ����ʾ", MessageType.None);
            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("��̾����ʾ", MessageType.Info);
            EditorGUILayout.HelpBox("������ʾ", MessageType.Warning);
            EditorGUILayout.HelpBox("���������ʾ", MessageType.Error);
        }
        #endregion

        #region Lesson11 �������߿ؼ�������API
        isCurveHide = EditorGUILayout.Foldout(isCurveHide, "�������߿ؼ�������API", true);
        if (isCurveHide)
        {
            curve = EditorGUILayout.CurveField("���߿ؼ�", curve);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("123123");
            EditorGUILayout.LabelField("123123");
            EditorGUILayout.LabelField("123123");
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #endregion

        #region EditorGuiUtility������
        #region Lesson13 ��Դ����
        #region ֪ʶ��һ Editor Default Resources�ļ���
        //Editor Default Resources Ҳ��Unity���е�һ�������ļ���
        //������Ҫ�����Ƿ����ṩ�� EditorGUIUtility ���ص���Դ
        //��Ҫʹ��EditorGUIUtility��������������Դ
        //������Ҫ����Դ������ Editor Default Resources �ļ�����
        #endregion
        #region ֪ʶ��� ������Դ�������Դ�����ڷ���null)
        //��ӦAPI��
        //EditorGUIUtility.Load
        //ע�����
        //1.ֻ�ܼ���Assets/Editor Default Resources/�ļ����µ���Դ
        //2.������Դʱ����Ҫ��д��Դ��׺��
        #endregion
        #region ֪ʶ���� ������Դ�������Դ�����ڻ�ֱ�ӱ���
        //��ӦAPI��
        //EditorGUIUtility.LoadRequired
        //ע�����
        //1.ֻ�ܼ���Assets/Editor Default Resources/�ļ����µ���Դ
        //2.������Դʱ����Ҫ��д��Դ��׺��
        #endregion
        isLoadHide = EditorGUILayout.Foldout(isLoadHide, "��Դ��������", true);
        if (isLoadHide)
        {
            if (GUILayout.Button("���ر༭��ͼƬ��Դ"))
            {
                img = EditorGUIUtility.Load("EditorTeach.png") as Texture;
            }
            if (img != null)
            {
                GUI.DrawTexture(new Rect(0, 300, 160, 90), img);
            }
        }
        #endregion

        #region Lesson 14 ��������ѯ,����ѡ����ʾ
        isFindHide = EditorGUILayout.Foldout(isFindHide, "��������ѯ,����ѡ����ʾ", true);
        if (isFindHide)
        {
            if (GUILayout.Button("���������ѯ����"))
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

            if (GUILayout.Button("����ѡ�ж���"))
            {
                if (img3 != null)
                    EditorGUIUtility.PingObject(img3);
            }
        }
        #endregion

        #region Lesson 15 �����¼����ݣ�����ת��
        isSendHide = EditorGUILayout.Foldout(isSendHide, "�����¼����ݣ�����ת��", true);
        if (isSendHide)
        {
            if (GUILayout.Button("�����¼�"))
            {
                Event e = EditorGUIUtility.CommandEvent("�ν������¼�");
                Lesson1 win = EditorWindow.GetWindow<Lesson1>();
                win.SendEvent(e);
                //�����¼�ʱ�����Զ������ܵĴ��ڴ�
            }


        }

        #endregion

        #region Lesson16 ָ������ʹ�ö�Ӧ���ָ��
        isCursorHide = EditorGUILayout.Foldout(isCursorHide, "ָ������ʹ�ö�Ӧָ��", true);
        if (isCursorHide)
        {
            EditorGUI.DrawRect(new Rect(0, 250, 100, 100), Color.yellow);
            EditorGUIUtility.AddCursorRect(new Rect(0,250,100,100),MouseCursor.Text);
        }

        #endregion

        #region Lesson17 ����ɫ�壬��������
        isColorHide = EditorGUILayout.Foldout(isColorHide, "����ɫ�壬��������", true);
        if (isColorHide)
        {
            EditorGUIUtility.DrawColorSwatch(new Rect(0, 250, 100, 100), color);
            EditorGUIUtility.DrawCurveSwatch(new Rect(100,250,100,100),curve,null,Color.red,Color.white);
        }
        #endregion
        #endregion

        #region Selection������

        #endregion

        EditorGUILayout.EndScrollView();
    }


    void Start()
    {
        #region ֪ʶ��һ EditorGUIUtility��������������ʲô�ģ�
        // Utility��ʵ�õ���˼��EditorGUIUtility �� EditorGUI �е�һ��ʵ�ù�����
        // �ṩ��һЩ EditorGUI ��ص���������API
        // ����ֻ��Ҫѧϰ���е���Գ��õ�����

        // �ٷ��ĵ���https://docs.unity3d.com/ScriptReference/EditorGUIUtility.html
        #endregion

        #region ֪ʶ��� ׼������
        //����һ���Զ���༭������ ����֮��ѧϰEditorGUIUtility��ص�֪ʶ
        #endregion
    }
}
