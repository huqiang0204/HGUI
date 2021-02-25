using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace huqiang.Pool
{
    public class History
    {
        public string type;
        public string name;
        public int point = -1;
        public List<int> operation = new List<int>();
        public List<byte[]> records = new List<byte[]>();
    }
    public class HistoryManager
    {
        class DataInfoContext
        {
            public string name;
            public DataInfo dataInfo;
        }
        public static string WorkPath;
        static List<History> historys = new List<History>();
        public void ClearAll()
        {
            historys.Clear();
        }
        public static void AddNewRecord(string type, string name, byte[] dat)
        {
            for (int i = 0; i < historys.Count; i++)
            {
                if (historys[i].type == type)
                    if (historys[i].name == name)
                    {
                        return;
                    }
            }
            History history = new History();
            history.type = type;
            history.name = name;
            history.records.Add(dat);
            history.operation.Add(0);
            historys.Add(history);
        }
        /// <summary>
        /// 添加一条记录
        /// </summary>
        public static void AddRecord(string type, string name, byte[] dat)
        {
            for (int i = 0; i < historys.Count; i++)
            {
                var item = historys[i];
                if (item.type == type)
                    if (item.name == name)
                    {
                        int c = item.records.Count;
                        int oc = item.operation.Count;
                        if (item.point < oc - 1)
                        {
                            for (int j = oc - 2; j >= item.point; j--)
                            {
                                if (j >= 0)
                                    item.operation.Add(item.operation[j]);
                            }
                        }
                        item.records.Add(dat);
                        item.operation.Add(c);
                        item.point = item.operation.Count - 1;
                        return;
                    }
            }
            History history = new History();
            history.type = type;
            history.name = name;
            history.records.Add(dat);
            historys.Add(history);
        }
        public static byte[] Undo(string type, string name, ref int count)
        {
            count = 0;
            for (int i = 0; i < historys.Count; i++)
            {
                var item = historys[i];
                if (item.type == type)
                    if (item.name == name)
                    {
                        if (item.point > 0)
                        {
                            item.point--;
                            count = item.point;
                            int p = item.operation[item.point];
                            return historys[i].records[p];
                        }
                        return null;
                    }
            }
            return null;
        }
        public static byte[] Redo(string type, string name, ref int count)
        {
            count = 0;
            for (int i = 0; i < historys.Count; i++)
            {
                var item = historys[i];
                if (item.type == type)
                    if (item.name == name)
                    {
                        int max = item.operation.Count;
                        if (item.point < max - 1)
                        {
                            item.point++;
                            count = max - item.point;
                            return item.records[item.operation[item.point]];
                        }
                        return null;
                    }
            }
            return null;
        }
        public static byte[] GetLastRecord(string type, string name)
        {
            for (int i = 0; i < historys.Count; i++)
            {
                var item = historys[i];
                if (item.type == type)
                    if (item.name == name)
                    {
                        if (item.point > 0)
                        {
                            int p = item.point;
                            p--;
                            p = item.operation[p];
                            return historys[i].records[p];
                        }
                        return null;
                    }
            }
            return null;
        }

        static List<DataInfoContext> contextList = new List<DataInfoContext>();
        public static DataInfo GetDataInfo(string name, bool isUnsafe, params Type[] types)
        {
            for (int i = 0; i < contextList.Count; i++)
            {
                if (contextList[i].name == name)
                    return contextList[i].dataInfo;
            }
            DataInfo di = new DataInfo();
            di.CreateConstruction = true;
            di.Unsafe = isUnsafe;
            for (int i = 0; i < types.Length; i++)
                di.Analysis(types[i]);
            DataInfoContext info = new DataInfoContext();
            info.name = name;
            info.dataInfo = di;
            contextList.Add(info);
            return di;
        }
    }
}

