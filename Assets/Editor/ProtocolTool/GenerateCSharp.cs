using System;
using System.IO;
using System.Xml;
using UnityEngine;


public class GenerateCSharp
{
    //Э�鱣��·��
    private string SAVE_PATH = Application.dataPath + "/Scripts/Protocol/";

    /// <summary>
    /// ����ö��
    /// </summary>
    /// <param name="nodes">xml�ڵ�</param>
    public void GenerateEnum(XmlNodeList nodes)
    {
        string namespaceStr = "";
        string enumNameStr = "";
        string fieldStr = "";

        foreach (XmlNode enumNode in nodes)
        {
            //�����ռ���
            namespaceStr = enumNode.Attributes["namespace"].Value;
            //ö����
            enumNameStr = enumNode.Attributes["name"].Value;
            //��ȡ�����ֶνڵ�
            XmlNodeList enumFields = enumNode.SelectNodes("field");
            //һ���µ�ö����Ҫ�����һ��ƴ�ӵ��ֶ��ַ���
            fieldStr = "";
            foreach (XmlNode enumField in enumFields)
            {
                //�������Ʊ����ʽ������
                fieldStr += "\t\t" + enumField.Attributes["name"].Value;
                if (enumField.InnerText != "")
                {
                    fieldStr += " = " + enumField.InnerText;
                }
                //����
                fieldStr += ",\r\n";
            }
            //�����пɱ����ݽ���ƴ��
            string enumStr = $"namespace {namespaceStr}\r\n" +
                            "{\r\n" +
                            $"\tpublic enum {enumNameStr}\r\n" +
                            "\t{\r\n" +
                                $"{fieldStr}" +
                            "\t}\r\n" +
                            "}";
            //�����ļ�·��
            string path = SAVE_PATH + namespaceStr + "/Enum/";
            //�������ļ����򴴽�
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //�ַ������� �洢Ϊö�ٽű��ļ�
            File.WriteAllText(path + enumNameStr + ".cs", enumStr);
        }
        Debug.Log("ö�����ɽ���");
    }

    /// <summary>
    /// �������ݽṹ��
    /// </summary>
    /// <param name="nodes">xml�ڵ�</param>
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
            //�����ռ���
            namespaceStr = dataNode.Attributes["namespace"].Value;
            //���ݽṹ��
            classNameStr = dataNode.Attributes["name"].Value;
            //��ȡ�����ֶνڵ�
            XmlNodeList fields = dataNode.SelectNodes("field");
            //ͨ������������г�Ա����������ƴ�� ����ƴ�ӽ��
            fieldStr = GetFieldStr(fields);
            //ͨ��������� ��GetBytesNum�����е��ַ�������ƴ��
            getBytesNumStr = GetGetBytesNumStr(fields);
            //ͨ��������� ��WriteBytes�����е��ַ�������ƴ��
            writeBytesStr = GetWriteBytesStr(fields);
            //ͨ��������� ��WriteBytes�����е��ַ�������ƴ��
            readBytesStr = GetReadBytesStr(fields);
            //�����еĿɱ����ݽ���һ��ƴ��
            //Ҫ�ǵü����������ռ�
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
            //�����ļ�·��
            string path = SAVE_PATH + namespaceStr + "/Data/";
            //�������ļ����򴴽�
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //�ַ������� �洢Ϊö�ٽű��ļ�
            File.WriteAllText(path + classNameStr + ".cs", dataStr);
        }
        Debug.Log("���ݽṹ�������ɽ���");
    }

    /// <summary>
    /// ������Ϣ��
    /// </summary>
    /// <param name="nodes">xml�ڵ�</param>
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
            //�����ռ���
            namespaceStr = msgNode.Attributes["namespace"].Value;
            //���ݽṹ��
            classNameStr = msgNode.Attributes["name"].Value;
            //��ȡ�����ֶνڵ�
            XmlNodeList fields = msgNode.SelectNodes("field");
            //ͨ������������г�Ա����������ƴ�� ����ƴ�ӽ��
            fieldStr = GetFieldStr(fields);
            //ͨ��������� ��GetBytesNum�����е��ַ�������ƴ��
            getBytesNumStr = GetGetBytesNumStr(fields);
            //ͨ��������� ��WriteBytes�����е��ַ�������ƴ��
            writeBytesStr = GetWriteBytesStr(fields);
            //ͨ��������� ��WriteBytes�����е��ַ�������ƴ��
            readBytesStr = GetReadBytesStr(fields);
            //�����еĿɱ����ݽ���һ��ƴ��
            //Ҫ�ǵü����������ռ�
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
                                    "\t\t\tint num = 8;\r\n" + //8������ϢID4�ֽ� + ��Ϣ�峤��4�ֽ�
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
            //�����ļ�·��
            string path = SAVE_PATH + namespaceStr + "/Msg/";
            //�������ļ����򴴽�
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //�ַ������� �洢Ϊö�ٽű��ļ�
            File.WriteAllText(path + classNameStr + ".cs", dataStr);
        }
        Debug.Log("��Ϣ�������ɽ���");
    }

    /// <summary>
    /// ��ȡ��Ա������������
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    private string GetFieldStr(XmlNodeList fields)
    {
        string fieldStr = "";

        foreach (XmlNode field in fields)
        {
            //��������
            string type = field.Attributes["type"].Value;
            //������
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
    /// ��ȡ���ݽṹ��Ա�����ֽ��ܳ���
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
                bytesNumStr += "\t\t\tnum += 2;\r\n"; //��shortȥ�洢List����
                bytesNumStr += $"\t\t\tfor (int i = 0; i < {name}.Count; ++i )\r\n";
                bytesNumStr += $"\t\t\t\tnum += {GetValueBytesNum(T, name + "[i]")};\r\n";
            }
            else if (type == "array")
            {
                string T = field.Attributes["T"].Value;
                bytesNumStr += "\t\t\tnum += 2;\r\n"; //��shortȥ�洢List����
                bytesNumStr += $"\t\t\tfor (int i = 0; i < {name}.Length; ++i )\r\n";
                bytesNumStr += $"\t\t\t\tnum += {GetValueBytesNum(T, name + "[i]")};\r\n";
            }
            else if (type == "Dictionary")
            {
                string TKey = field.Attributes["TKey"].Value;
                string TValue = field.Attributes["TValue"].Value;
                bytesNumStr += "\t\t\tnum += 2;\r\n"; //��shortȥ�洢List����
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
    /// ����ָ�����ͱ������ֽڳ���
    /// </summary>
    /// <param name="type">����������</param>
    /// <param name="name">����������</param>
    /// <returns></returns>
    private string GetValueBytesNum(string type, string name)
    {
        //�ɽ�������
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
    /// ƴ��д���ֽں����ķ���
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
    /// �������Ͳ�ͬʹ�ò�ͬ���ֽ�д�뷽��
    /// </summary>
    /// <param name="type">��������</param>
    /// <param name="name">������</param>
    /// <returns></returns>
    private string GetFieldWriteBytesStr(string type, string name)
    {
        //�ɽ�������
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
    /// ƴ�Ӷ�ȡ�ֽں����ķ���
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
    /// �������Ͳ�ͬʹ�ò�ͬ���ֽڶ�ȡ����
    /// </summary>
    /// <param name="type">��������</param>
    /// <param name="name">������</param>
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
