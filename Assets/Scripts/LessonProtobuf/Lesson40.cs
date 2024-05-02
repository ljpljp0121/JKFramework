
using GamePlayerTest;
using Google.Protobuf;
using System.IO;
using UnityEngine;

public class Lesson40 : MonoBehaviour
{
    void Start()
    {
        TestMsg msg = new TestMsg();
        msg.ListInt.Add(1);
        msg.TestBool = false;
        msg.TestD = 5.5;
        msg.TestU32 = 99;
        msg.TestMap.Add(1, "ljp");

        print(Application.persistentDataPath);
        using (FileStream fs = File.Create(Application.persistentDataPath + "/TestMsg.liao"))
        {
            msg.WriteTo(fs);
        }
        TestMsg testMsg2 = null;
        //∑¥–Ú¡–ªØ
        using (FileStream fs = File.OpenRead(Application.persistentDataPath + "/TestMsg.liao"))
        {
            testMsg2 = TestMsg.Parser.ParseFrom(fs);
        }
        print(testMsg2.TestMap[1]);
        print(testMsg2.ListInt[0]);
        print(testMsg2.TestInt);

        byte[] bytes = null;
        using (MemoryStream ms = new MemoryStream())
        {
            msg.WriteTo(ms);
            bytes = ms.ToArray();
            print(bytes.Length);
        }
        testMsg2 = null;
        using (MemoryStream ms = new MemoryStream(bytes))
        {
            testMsg2 = TestMsg.Parser.ParseFrom(ms);
        }

        print(testMsg2.TestMap[1]);
        print(testMsg2.ListInt[0]);
        print(testMsg2.TestInt);
    }

    void Update()
    {

    }
}
