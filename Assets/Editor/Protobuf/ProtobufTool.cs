using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;


public class ProtobufTool
{
    //协议配置文件路径
    private static string PROTO_PATH = "E:\\Unity\\projects\\JKFramework\\Protobuf\\proto";
    //协议生成可执行文件路径
    private static string PROTOC_PATH = "E:\\Unity\\projects\\JKFramework\\Protobuf\\protoc.exe";
    //C#文件存储路径
    private static string CSHARP_PATH = "E:\\Unity\\projects\\JKFramework\\Protobuf\\csharp";

    private static string CPP_PATH = "E:\\Unity\\projects\\JKFramework\\Protobuf\\cpp";

    private static string JAVA_PATH = "E:\\Unity\\projects\\JKFramework\\Protobuf\\java";

    [MenuItem("ProtobufTool/生成C#代码")]
    private static void GenerateCSharp()
    {
        Generate("csharp_out",CSHARP_PATH);
    }

    [MenuItem("ProtobufTool/生成C++代码")]
    private static void GenerateCPP()
    {
        Generate("cpp_out",CPP_PATH);
    }

    [MenuItem("ProtobufTool/生成Java代码")]
    private static void GenerateJava()
    {
        Generate("java_out", JAVA_PATH);
    }

    //生成对应脚本的方法
    private static void Generate(string outCmd,string outPath)
    {
        //遍历对应协议配置文件夹 
        DirectoryInfo directoryInfo = Directory.CreateDirectory(PROTO_PATH);
        FileInfo[] files = directoryInfo.GetFiles();

        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Extension == ".proto")
            {
                Process cmd = new Process();

                cmd.StartInfo.FileName = PROTOC_PATH;
                //命令
                cmd.StartInfo.Arguments = $"-I={PROTO_PATH} --{outCmd}={outPath} {files[i]}";

                cmd.Start();

                UnityEngine.Debug.Log(files[i] + "生成结束");
            }
        }
    }
}
