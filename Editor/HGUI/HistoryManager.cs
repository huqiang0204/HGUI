using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    class DataWriterInfo
    {
        public string name;
        public DataWriter writer;
    }
    static List<DataWriterInfo> writers = new List<DataWriterInfo>();
    public static DataWriter GetDwriter<T>(string name) where T : new()
    {
        for (int i = 0; i < writers.Count; i++)
        {
            if (writers[i].name == name)
                return writers[i].writer;
        }
        DataWriter writer = new DataWriter();
        writer.Analysis<T>();
        DataWriterInfo info = new DataWriterInfo();
        info.writer = writer;
        info.name = name;
        writers.Add(info);
        return writer;
    }
    class DataReaderInfo
    {
        public string name;
        public DataReader reader;
    }
    static List<DataReaderInfo> readers = new List<DataReaderInfo>();
    public static DataReader GetDReader<T>(string name) where T : new()
    {
        for (int i = 0; i < readers.Count; i++)
        {
            if (readers[i].name == name)
                return readers[i].reader;
        }
        DataReader writer = new DataReader();
        writer.Analysis<T>();
        DataReaderInfo info = new DataReaderInfo();
        info.reader = writer;
        info.name = name;
        readers.Add(info);
        return writer;
    }
}
