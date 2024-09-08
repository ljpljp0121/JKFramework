using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;


public class ProtobufTool
{
    //Э�������ļ�·��
    private static string PROTO_PATH = "E:\\Unity\\projects\\JKFramework\\Protobuf\\proto";
    //Э�����ɿ�ִ���ļ�·��
    private static string PROTOC_PATH = "E:\\Unity\\projects\\JKFramework\\Protobuf\\protoc.exe";
    //C#�ļ��洢·��
    private static string CSHARP_PATH = "E:\\Unity\\projects\\JKFramework\\Protobuf\\csharp";

    private static string CPP_PATH = "E:\\Unity\\projects\\JKFramework\\Protobuf\\cpp";

    private static string JAVA_PATH = "E:\\Unity\\projects\\JKFramework\\Protobuf\\java";

    [MenuItem("ProtobufTool/����C#����")]
    private static void GenerateCSharp()
    {
        Generate("csharp_out",CSHARP_PATH);
    }

    [MenuItem("ProtobufTool/����C++����")]
    private static void GenerateCPP()
    {
        Generate("cpp_out",CPP_PATH);
    }

    [MenuItem("ProtobufTool/����Java����")]
    private static void GenerateJava()
    {
        Generate("java_out", JAVA_PATH);
    }

    //���ɶ�Ӧ�ű��ķ���
    private static void Generate(string outCmd,string outPath)
    {
        //������ӦЭ�������ļ��� 
        DirectoryInfo directoryInfo = Directory.CreateDirectory(PROTO_PATH);
        FileInfo[] files = directoryInfo.GetFiles();

        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Extension == ".proto")
            {
                Process cmd = new Process();

                cmd.StartInfo.FileName = PROTOC_PATH;
                //����
                cmd.StartInfo.Arguments = $"-I={PROTO_PATH} --{outCmd}={outPath} {files[i]}";

                cmd.Start();

                UnityEngine.Debug.Log(files[i] + "���ɽ���");
            }
        }
    }
}
