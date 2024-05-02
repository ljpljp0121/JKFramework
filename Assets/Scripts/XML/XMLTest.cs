using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class XMLTest : MonoBehaviour
{    

    void Start()
    {
        PlayerInfo playerInfo = new PlayerInfo();

        string path = Application.persistentDataPath + "/TestLesson3.xml";
        //序列化
        using (StreamWriter writer = new StreamWriter(path))
        {
            XmlSerializer s = new XmlSerializer(typeof(PlayerInfo));

            s.Serialize(writer, playerInfo);
        }
        //反序列化
        using(StreamReader reader = new StreamReader(path))
        {
            XmlSerializer s = new XmlSerializer (typeof(PlayerInfo));

            PlayerInfo playerInfo1 = s.Deserialize(reader) as PlayerInfo;
        }

        
    }

    void Update()
    {
        
    }
}
