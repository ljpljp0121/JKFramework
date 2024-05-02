using System;
using System.IO;
using System.Xml;
using UnityEngine;


public class GenerateCSharp
{
    //协议保存路径
    private string SAVE_PATH = Application.dataPath + "/Scripts/Protocol/";

    /// <summary>
    /// 生成枚举
    /// </summary>
    /// <param name="nodes">xml节点</param>
    public void GenerateEnum(XmlNodeList nodes)
    {
        string namespaceStr = "";
        string enumNameStr = "";
        string fieldStr = "";

        foreach (XmlNode enumNode in nodes)
        {
            //命名空间名
            namespaceStr = enumNode.Attributes["namespace"].Value;
            //枚举名
            enumNameStr = enumNode.Attributes["name"].Value;
            //获取所有字段节点
            XmlNodeList enumFields = enumNode.SelectNodes("field");
            //一个新的枚举需要清空上一次拼接的字段字符串
            fieldStr = "";
            foreach (XmlNode enumField in enumFields)
            {
                //空两个制表符格式更美观
                fieldStr += "\t\t" + enumField.Attributes["name"].Value;
                if (enumField.InnerText != "")
                {
                    fieldStr += " = " + enumField.InnerText;
                }
                //空行
                fieldStr += ",\r\n";
            }
            //对所有可变内容进行拼接
            string enumStr = $"namespace {namespaceStr}\r\n" +
                            "{\r\n" +
                            $"\tpublic enum {enumNameStr}\r\n" +
                            "\t{\r\n" +
                                $"{fieldStr}" +
                            "\t}\r\n" +
                            "}";
            //保存文件路径
            string path = SAVE_PATH + namespaceStr + "/Enum/";
            //不存在文件夹则创建
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //字符串保存 存储为枚举脚本文件
            File.WriteAllText(path + enumNameStr + ".cs", enumStr);
        }
        Debug.Log("枚举生成结束");
    }

    /// <summary>
    /// 生成数据结构类
    /// </summary>
    /// <param name="nodes">xml节点</param>
    public void GenerateData(XmlNodeList nodes)
    {
        string namespaceStr = "";
        string classNameStr = "";
        string fieldStr = "";

        string getBytesNumStr = "";
        string writeBytesStr = "";
        string readBytesStr = "";
        foreach (XmlNode dataNode in nodes)
        {
            //命名空间名
            namespaceStr = dataNode.Attributes["namespace"].Value;
            //数据结构名
            classNameStr = dataNode.Attributes["name"].Value;
            //获取所有字段节点
            XmlNodeList fields = dataNode.SelectNodes("field");
            //通过这个方法进行成员变量声明的拼接 返回拼接结果
            fieldStr = GetFieldStr(fields);
            //通过这个方法 对GetBytesNum函数中的字符串进行拼接
            getBytesNumStr = GetGetBytesNumStr(fields);
            //通过这个方法 对WriteBytes函数中的字符串进行拼接
            writeBytesStr = GetWriteBytesStr(fields);
            //通过这个方法 对WriteBytes函数中的字符串进行拼接
            readBytesStr = GetReadBytesStr(fields);
            //对所有的可变内容进行一次拼接
            //要记得加引用命名空间
            string dataStr = "using System;\r\n" +
                            "using System.Collections.Generic;\r\n" +
                            "using System.Text;\r\n\r\n" +
                            $"namespace {namespaceStr}\r\n" +
                            "{\r\n" +
                            $"\tpublic class {classNameStr} : BaseData\r\n" +
                            "\t{\r\n" +
                                $"{fieldStr}" +
                                "\t\tpublic override int GetBytesNum()\r\n" +
                                "\t\t{\t\n" +
                                    "\t\t\tint num = 0;\r\n" +
                                    $"{getBytesNumStr}" +
                                    "\t\t\treturn num;\r\n" +
                                "\t\t}\r\n" +
                                "\t\tpublic override byte[] WriteBytes()\r\n" +
                                "\t\t{\r\n" +
                                    "\t\t\tint index = 0;\r\n" +
                                    "\t\t\tbyte[] bytes = new byte[GetBytesNum()];\r\n" +
                                    $"{writeBytesStr}" +
                                    "\t\t\treturn bytes;\r\n" +
                                "\t\t}\r\n" +
                                "\t\tpublic override int ReadBytes(byte[] bytes,int beginIndex = 0)\r\n" +
                                "\t\t{\r\n" +
                                    "\t\t\tint index = beginIndex;\r\n" +
                                    $"{readBytesStr}" +
                                    "\t\t\treturn index - beginIndex;\r\n" +
                                "\t\t}\r\n" +
                            "\t}\r\n" +
                            "}";
            //保存文件路径
            string path = SAVE_PATH + namespaceStr + "/Data/";
            //不存在文件夹则创建
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //字符串保存 存储为枚举脚本文件
            File.WriteAllText(path + classNameStr + ".cs", dataStr);
        }
        Debug.Log("数据结构类型生成结束");
    }

    /// <summary>
    /// 生成消息类
    /// </summary>
    /// <param name="nodes">xml节点</param>
    public void GenerateMsg(XmlNodeList nodes)
    {
        string idStr = "";
        string namespaceStr = "";
        string classNameStr = "";
        string fieldStr = "";

        string getBytesNumStr = "";
        string writeBytesStr = "";
        string readBytesStr = "";
        foreach (XmlNode msgNode in nodes)
        {
            idStr = msgNode.Attributes["id"].Value;
            //命名空间名
            namespaceStr = msgNode.Attributes["namespace"].Value;
            //数据结构名
            classNameStr = msgNode.Attributes["name"].Value;
            //获取所有字段节点
            XmlNodeList fields = msgNode.SelectNodes("field");
            //通过这个方法进行成员变量声明的拼接 返回拼接结果
            fieldStr = GetFieldStr(fields);
            //通过这个方法 对GetBytesNum函数中的字符串进行拼接
            getBytesNumStr = GetGetBytesNumStr(fields);
            //通过这个方法 对WriteBytes函数中的字符串进行拼接
            writeBytesStr = GetWriteBytesStr(fields);
            //通过这个方法 对WriteBytes函数中的字符串进行拼接
            readBytesStr = GetReadBytesStr(fields);
            //对所有的可变内容进行一次拼接
            //要记得加引用命名空间
            string dataStr = "using System;\r\n" +
                            "using System.Collections.Generic;\r\n" +
                            "using System.Text;\r\n\r\n" +
                            $"namespace {namespaceStr}\r\n" +
                            "{\r\n" +
                            $"\tpublic class {classNameStr} : BaseInfo\r\n" +
                            "\t{\r\n" +
                                $"{fieldStr}" +
                                "\t\tpublic override int GetBytesNum()\r\n" +
                                "\t\t{\t\n" +
                                    "\t\t\tint num = 8;\r\n" + //8代表消息ID4字节 + 消息体长度4字节
                                    $"{getBytesNumStr}" +
                                    "\t\t\treturn num;\r\n" +
                                "\t\t}\r\n" +
                                "\t\tpublic override byte[] WriteBytes()\r\n" +
                                "\t\t{\r\n" +
                                    "\t\t\tint index = 0;\r\n" +
                                    "\t\t\tbyte[] bytes = new byte[GetBytesNum()];\r\n" +
                                    $"\t\t\tWriteInt(bytes, GetID(), ref index);\r\n"+
                                    $"\t\t\tWriteInt(bytes, bytes.Length - 8, ref index);\r\n" +
                                    $"{writeBytesStr}" +
                                    "\t\t\treturn bytes;\r\n" +
                                "\t\t}\r\n" +
                                "\t\tpublic override int ReadBytes(byte[] bytes,int beginIndex = 0)\r\n" +
                                "\t\t{\r\n" +
                                    "\t\t\tint index = beginIndex;\r\n" +
                                    $"{readBytesStr}" +
                                    "\t\t\treturn index - beginIndex;\r\n" +
                                "\t\t}\r\n" +
                                "\t\tpublic override int GetID()\r\n" +
                                "\t\t{\r\n" +
                                    $"\t\t\treturn {idStr};\r\n" +
                                "\t\t}\r\n" +
                            "\t}\r\n" +
                            "}";
            //保存文件路径
            string path = SAVE_PATH + namespaceStr + "/Msg/";
            //不存在文件夹则创建
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //字符串保存 存储为枚举脚本文件
            File.WriteAllText(path + classNameStr + ".cs", dataStr);
        }
        Debug.Log("消息类型生成结束");
    }

    /// <summary>
    /// 获取成员变量声明内容
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    private string GetFieldStr(XmlNodeList fields)
    {
        string fieldStr = "";

        foreach (XmlNode field in fields)
        {
            //变量类型
            string type = field.Attributes["type"].Value;
            //变量名
            string fieldName = field.Attributes["name"].Value;
            if (type == "List")
            {
                string T = field.Attributes["T"].Value;
                fieldStr += $"\t\tpublic List<{T}> ";
            }
            else if (type == "array")
            {
                string T = field.Attributes["T"].Value;
                fieldStr += $"\t\tpublic {T}[] ";
            }
            else if (type == "Dictionary")
            {
                string TKey = field.Attributes["TKey"].Value;
                string TValue = field.Attributes["TValue"].Value;
                fieldStr += $"\t\tpublic Dictionary<{TKey},{TValue}> ";
            }
            else if (type == "enum")
            {
                string T = field.Attributes["T"].Value;
                fieldStr += $"\t\tpublic {T} ";
            }
            else
            {
                fieldStr += $"\t\tpublic {type} ";
            }

            fieldStr += $"{fieldName};\r\n";
        }

        return fieldStr;
    }

    /// <summary>
    /// 获取数据结构成员变量字节总长度
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    private string GetGetBytesNumStr(XmlNodeList fields)
    {
        string bytesNumStr = "";

        string type = "";
        string name = "";
        foreach (XmlNode field in fields)
        {
            type = field.Attributes["type"].Value;
            name = field.Attributes["name"].Value;
            if (type == "List")
            {
                string T = field.Attributes["T"].Value;
                bytesNumStr += "\t\t\tnum += 2;\r\n"; //用short去存储List长度
                bytesNumStr += $"\t\t\tfor (int i = 0; i < {name}.Count; ++i )\r\n";
                bytesNumStr += $"\t\t\t\tnum += {GetValueBytesNum(T, name + "[i]")};\r\n";
            }
            else if (type == "array")
            {
                string T = field.Attributes["T"].Value;
                bytesNumStr += "\t\t\tnum += 2;\r\n"; //用short去存储List长度
                bytesNumStr += $"\t\t\tfor (int i = 0; i < {name}.Length; ++i )\r\n";
                bytesNumStr += $"\t\t\t\tnum += {GetValueBytesNum(T, name + "[i]")};\r\n";
            }
            else if (type == "Dictionary")
            {
                string TKey = field.Attributes["TKey"].Value;
                string TValue = field.Attributes["TValue"].Value;
                bytesNumStr += "\t\t\tnum += 2;\r\n"; //用short去存储List长度
                bytesNumStr += $"\t\t\tforeach ({TKey} key in {name}.Keys)\r\n";
                bytesNumStr += "\t\t\t{\r\n";
                bytesNumStr += $"\t\t\t\tnum += {GetValueBytesNum(TKey, "key")};\r\n";
                bytesNumStr += $"\t\t\t\tnum += {GetValueBytesNum(TValue, name + "[key]")};\r\n";
                bytesNumStr += "\t\t\t}\r\n";
            }
            else
            {
                bytesNumStr += $"\t\t\tnum += {GetValueBytesNum(type, name)};\r\n";
            }
        }

        return bytesNumStr;
    }

    /// <summary>
    /// 返回指定类型变量的字节长度
    /// </summary>
    /// <param name="type">变量的类型</param>
    /// <param name="name">变量的名字</param>
    /// <returns></returns>
    private string GetValueBytesNum(string type, string name)
    {
        //可接着增加
        switch (type)
        {
            case "int":
            case "float":
            case "enum":
                return "4";
            case "byte":
            case "bool":
                return "1";
            case "short":
                return "2";
            case "double":
            case "long":
                return "8";
            case "string":
                return $"4 + Encoding.UTF8.GetByteCount({name})";
            default:
                return name + ".GetBytesNum()";
        }
    }

    /// <summary>
    /// 拼接写入字节函数的方法
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private string GetWriteBytesStr(XmlNodeList fields)
    {
        string writeBytesStr = "";

        string type = "";
        string name = "";

        foreach (XmlNode field in fields)
        {
            type = field.Attributes["type"].Value;
            name = field.Attributes["name"].Value;
            if (type == "List")
            {
                string T = field.Attributes["T"].Value;
                writeBytesStr += $"\t\t\tWriteShort(bytes, (short){name}.Count, ref index);\r\n";
                writeBytesStr += $"\t\t\tfor (int i = 0; i < {name}.Count; ++i)\r\n";
                writeBytesStr += $"\t\t\t\t{GetFieldWriteBytesStr(T, name + "[i]")}\r\n";
            }
            else if (type == "array")
            {
                string T = field.Attributes["T"].Value;
                writeBytesStr += $"\t\t\tWriteShort(bytes, (short){name}.Length, ref index);\r\n";
                writeBytesStr += $"\t\t\tfor (int i = 0; i < {name}.Length; ++i)\r\n";
                writeBytesStr += $"\t\t\t\t{GetFieldWriteBytesStr(T, name + "[i]")}\r\n";
            }
            else if (type == "Dictionary")
            {
                string TKey = field.Attributes["TKey"].Value;
                string TValue = field.Attributes["TValue"].Value;
                writeBytesStr += $"\t\t\tWriteShort(bytes, (short){name}.Count, ref index);\r\n";
                writeBytesStr += $"\t\t\tforeach ({TKey} key in {name}.Keys)\r\n";
                writeBytesStr += "\t\t\t{\r\n";
                writeBytesStr += $"\t\t\t\t{GetFieldWriteBytesStr(TKey, "key")}\r\n";
                writeBytesStr += $"\t\t\t\t{GetFieldWriteBytesStr(TValue, name + "[key]")}\r\n";
                writeBytesStr += "\t\t\t}\r\n";
            }
            else
            {
                writeBytesStr += "\t\t\t" + GetFieldWriteBytesStr(type, name) + "\r\n";
            }
        }

        return writeBytesStr;
    }

    /// <summary>
    /// 根据类型不同使用不同的字节写入方法
    /// </summary>
    /// <param name="type">变量类型</param>
    /// <param name="name">变量名</param>
    /// <returns></returns>
    private string GetFieldWriteBytesStr(string type, string name)
    {
        //可接着增加
        switch (type)
        {
            case "int":
                return $"WriteInt(bytes, {name}, ref index);";
            case "float":
                return $"WriteFloat(bytes, {name}, ref index);";
            case "enum":
                return $"WriteFloat(bytes, Convert.ToInt32({name}) , ref index);";
            case "byte":
                return $"WriteByte(bytes, {name}, ref index);";
            case "bool":
                return $"WriteBool(bytes, {name}, ref index);";
            case "short":
                return $"WriteShort(bytes, {name}, ref index);";
            case "long":
                return $"WriteLong(bytes, {name}, ref index);";
            case "string":
                return $"WriteString(bytes, {name}, ref index);";
            default:
                return $"WriteData(bytes, {name}, ref index);";
        }
    }

    /// <summary>
    /// 拼接读取字节函数的方法
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    private string GetReadBytesStr(XmlNodeList fields)
    {
        string readBytesStr = "";

        string type = "";
        string name = "";

        foreach (XmlNode field in fields)
        {
            type = field.Attributes["type"].Value;
            name = field.Attributes["name"].Value;
            if (type == "List")
            {
                string T = field.Attributes["T"].Value;
                readBytesStr += $"\t\t\t{name} = new List<{T}>();\r\n";
                readBytesStr += $"\t\t\tshort {name}Count = ReadShort(bytes, ref index);\r\n";
                readBytesStr += $"\t\t\tfor (int i = 0; i < {name}Count; ++i)\r\n";
                readBytesStr += $"\t\t\t\t{name}.Add({GetFieldReadBytesStr(T)});\r\n";
            }
            else if (type == "array")
            {
                string T = field.Attributes["T"].Value;
                readBytesStr += $"\t\t\tshort {name}Length = ReadShort(bytes, ref index);\r\n";
                readBytesStr += $"\t\t\t{name} = new {T}[{name}Length];\r\n";
                readBytesStr += $"\t\t\tfor (int i = 0; i < {name}Length; ++i)\r\n";
                readBytesStr += $"\t\t\t\t{name}[i] = {GetFieldReadBytesStr(T)};\r\n";
            }
            else if (type == "Dictionary")
            {
                string TKey = field.Attributes["TKey"].Value;
                string TValue = field.Attributes["TValue"].Value;
                readBytesStr += $"\t\t\t{name} = new Dictionary<{TKey},{TValue}>();\r\n";
                readBytesStr += $"\t\t\tshort {name}Count = ReadShort(bytes, ref index);\r\n";
                readBytesStr += $"\t\t\tfor (int i = 0; i < {name}Count; ++i)\r\n";
                readBytesStr += $"\t\t\t\t{name}.Add({GetFieldReadBytesStr(TKey)},{GetFieldReadBytesStr(TValue)});\r\n";
            }
            else if (type == "enum")
            {
                string T = field.Attributes["T"].Value;
                readBytesStr += $"\t\t\t{name} = ({T})ReadInt(bytes, ref index);\r\n";
            }
            else
            {
                readBytesStr += $"\t\t\t{name} = {GetFieldReadBytesStr(type)};\r\n";
            }
        }
        return readBytesStr;
    }

    /// <summary>
    /// 根据类型不同使用不同的字节读取方法
    /// </summary>
    /// <param name="type">变量类型</param>
    /// <param name="name">变量名</param>
    /// <returns></returns>
    private string GetFieldReadBytesStr(string type)
    {
        switch (type)
        {
            case "int":
                return $"ReadInt(bytes,ref index)";
            case "float":
                return $"ReadFloat(bytes,ref index)";
            case "enum":
                return $"ReadInt(bytes,ref index)";
            case "byte":
                return $"ReadByte(bytes,ref index)";
            case "bool":
                return $"ReadBool(bytes,ref index)";
            case "short":
                return $"ReadShort(bytes,ref index)";
            case "long":
                return $"ReadLong(bytes,ref index)";
            case "string":
                return $"ReadString(bytes,ref index)";
            default:
                return $"ReadData<{type}>(bytes,ref index)";
        }
    }
}
