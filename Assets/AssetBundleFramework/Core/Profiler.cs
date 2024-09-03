using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Text;

namespace AssetBundleFramework.Core
{
    public class Profiler
    {
        private static readonly Stopwatch ms_Stopwatch = Stopwatch.StartNew();
        private static readonly StringBuilder ms_StringBuilder = new StringBuilder();
        private static readonly List<Profiler> ms_Stack = new List<Profiler>();

        private List<Profiler> m_children;
        private string m_Name;
        private int m_level;
        private long m_Timestamp;
        private long m_Time;
        private int m_Count;
        public Profiler(string name)
        {
            m_children = null;
            m_Name = name;
            m_level = 0;
            m_Timestamp = -1;
            m_Time = 0;
            m_Count = 0;
        }

        private Profiler(string name, int level) : this(name)
        {
            m_level = level;
        }

        public Profiler CreateChild(string name)
        {
            if (m_children == null)
            {
                m_children = new List<Profiler>();
            }

            Profiler profiler = new Profiler(name, m_level + 1);
            m_children.Add(profiler);
            return profiler;
        }

        public void Start()
        {
            if (m_Timestamp != -1)
            {
                throw new Exception($"{nameof(Profiler)} {nameof(Start)} error,repeat start, name:{m_Name}");
            }
            m_Timestamp = ms_Stopwatch.ElapsedTicks;
        }

        public void Stop()
        {
            if (m_Timestamp == -1)
            {
                throw new Exception($"{nameof(Profiler)} {nameof(Start)} error,repeat stop, name:{m_Name}");
            }
            m_Time += ms_Stopwatch.ElapsedTicks - m_Timestamp;
            m_Count++;
            m_Timestamp = -1;
        }

        private void Format()
        {
            ms_StringBuilder.AppendLine();

            for (int i = 0; i < m_level; i++)
            {
                ms_StringBuilder.Append(i < m_level - 1 ? "|  " : "|--");
            }

            ms_StringBuilder.Append(m_Name);

            if (m_Count <= 0)
            {
                return;
            }

            ms_StringBuilder.Append(" [");
            ms_StringBuilder.Append("Count");
            ms_StringBuilder.Append(":");
            ms_StringBuilder.Append(m_Count);
            ms_StringBuilder.Append(", ");
            ms_StringBuilder.Append("Time");
            ms_StringBuilder.Append(": ");

            ms_StringBuilder.Append($"{(float)m_Time / TimeSpan.TicksPerMillisecond:F2}");
            ms_StringBuilder.Append("ºÁÃë    ");

            ms_StringBuilder.Append($"{(float)m_Time / TimeSpan.TicksPerSecond:F2}");
            ms_StringBuilder.Append("Ãë    ");

            ms_StringBuilder.Append($"{(float)m_Time / TimeSpan.TicksPerMinute:F2}");
            ms_StringBuilder.Append("·Ö    ");

            ms_StringBuilder.Append("]");
        }

        public override string ToString()
        {
            ms_StringBuilder.Clear();
            ms_Stack.Clear();
            ms_Stack.Add(this);

            while (ms_Stack.Count > 0)
            {
                int index = ms_Stack.Count - 1;
                Profiler profiler = ms_Stack[index];
                ms_Stack.RemoveAt(index);

                profiler.Format();

                List<Profiler> children = profiler.m_children;

                if (children == null)
                {
                    continue;
                }
                for (int i = children.Count - 1; i >= 0; i--)
                {
                    ms_Stack.Add(children[i]);
                }
            }

            return ms_StringBuilder.ToString();
        }
    }
}
