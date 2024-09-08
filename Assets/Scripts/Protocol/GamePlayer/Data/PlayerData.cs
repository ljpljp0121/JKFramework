using System;
using System.Collections.Generic;
using System.Text;

namespace GamePlayer
{
	public class PlayerData : BaseData
	{
		public int id;
		public float atk;
		public long lev;
		public int[] arrays;
		public List<int> list;
		public Dictionary<int,string> dict;
		public E_PLAYER_TYPE heroType;
		public override int GetBytesNum()
		{	
			int num = 0;
			num += 4;
			num += 4;
			num += 8;
			num += 2;
			for (int i = 0; i < arrays.Length; ++i )
				num += 4;
			num += 2;
			for (int i = 0; i < list.Count; ++i )
				num += 4;
			num += 2;
			foreach (int key in dict.Keys)
			{
				num += 4;
				num += 4 + Encoding.UTF8.GetByteCount(dict[key]);
			}
			num += 4;
			return num;
		}
		public override byte[] WriteBytes()
		{
			int index = 0;
			byte[] bytes = new byte[GetBytesNum()];
			WriteInt(bytes, id, ref index);
			WriteFloat(bytes, atk, ref index);
			WriteLong(bytes, lev, ref index);
			WriteShort(bytes, (short)arrays.Length, ref index);
			for (int i = 0; i < arrays.Length; ++i)
				WriteInt(bytes, arrays[i], ref index);
			WriteShort(bytes, (short)list.Count, ref index);
			for (int i = 0; i < list.Count; ++i)
				WriteInt(bytes, list[i], ref index);
			WriteShort(bytes, (short)dict.Count, ref index);
			foreach (int key in dict.Keys)
			{
				WriteInt(bytes, key, ref index);
				WriteString(bytes, dict[key], ref index);
			}
			WriteFloat(bytes, Convert.ToInt32(heroType) , ref index);
			return bytes;
		}
		public override int ReadBytes(byte[] bytes,int beginIndex = 0)
		{
			int index = beginIndex;
			id = ReadInt(bytes,ref index);
			atk = ReadFloat(bytes,ref index);
			lev = ReadLong(bytes,ref index);
			short arraysLength = ReadShort(bytes, ref index);
			arrays = new int[arraysLength];
			for (int i = 0; i < arraysLength; ++i)
				arrays[i] = ReadInt(bytes,ref index);
			list = new List<int>();
			short listCount = ReadShort(bytes, ref index);
			for (int i = 0; i < listCount; ++i)
				list.Add(ReadInt(bytes,ref index));
			dict = new Dictionary<int,string>();
			short dictCount = ReadShort(bytes, ref index);
			for (int i = 0; i < dictCount; ++i)
				dict.Add(ReadInt(bytes,ref index),ReadString(bytes,ref index));
			heroType = (E_PLAYER_TYPE)ReadInt(bytes, ref index);
			return index - beginIndex;
		}
	}
}