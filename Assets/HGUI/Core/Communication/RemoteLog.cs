using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Communication
{
    [Serializable]
    public class LogData
    {
        public string condition;
        public string stackTrace;
        public LogType type;
    }
    public class RemoteLog
    {
        static RemoteLog ins;
        public static RemoteLog Instance { get { if (ins == null) ins = new RemoteLog(); return ins; } }
        KcpLink link;
        public void Connection(string ip, int port)
        {
            var address = IPAddress.Parse(ip);
            var kcp = new KcpServer<KcpLink>(0);
            kcp.Run(1);
            link = kcp.FindOrCreateLink(new IPEndPoint(address, port));
            link.Send(0, new byte[1]);
            Application.logMessageReceived += Log;
        }
        public void Log(string condtion,string stack, LogType type)
        {
            LogData log = new LogData();
            log.condition = condtion;
            log.stackTrace = stack;
            log.type = type;
            var str = JsonUtility.ToJson(log);
            link.Send(EnvelopeType.String, Encoding.UTF8.GetBytes(str));
        }
        public void Dispose()
        {
            Application.logMessageReceived -= Log;
        }
    }
}
