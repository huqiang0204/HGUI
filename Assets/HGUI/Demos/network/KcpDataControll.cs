using huqiang;
using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Net
{
    public class Req
    {
        public const Int32 Cmd = 0;
        public const Int32 Type = 1;
        public const Int32 Error = 2;
        public const Int32 Args = 3;
        public const Int32 Length = 4;
    }
    public class MessageType
    {
        public const Int32 Background = -1;
        public const Int32 Def = 0;
        public const Int32 Rpc = 1;
        public const Int32 Query = 2;
        public const Int32 Game = 3;
    }
    [Serializable]
    public class LogData
    {
        public string condition;
        public string stackTrace;
        public LogType type;
    }
    public class KcpData
    {
        public byte[] dat;
        public byte tag;
    }
    public class KcpSocket : KcpLink
    {
        public QueueBuffer<KcpData> datas;
        public bool connect;
        public KcpSocket()
        {
            datas = new QueueBuffer<KcpData>();
        }
        public override void Dispatch(BlockInfo<byte> dat, byte tag)
        {
            KcpData data = new KcpData();
            data.tag = tag;
            unsafe
            {
                byte[] buf = new byte[dat.DataCount];
                byte* src = dat.Addr;
                for (int i = 0; i < buf.Length; i++)
                {
                    buf[i] = src[i];
                }
                data.dat = buf;
            }
            datas.Enqueue(data);
        }
        public override bool Disconnect()
        {

            return false;
        }
        public void ConnectionOK()
        {
            
        }
    }
    public class KcpDataControll
    {
        static KcpDataControll ins;
        public static KcpDataControll Instance { get { if (ins == null) ins = new KcpDataControll(); return ins; } }
        public bool Connected
        {
            get
            {
                if (link == null)
                    return false;
                return link.Connected;
            }
        }
        KcpSocket link;
        KcpSocket logLink;
        KcpServer<KcpSocket> server;
        public void Connection(string ip, int port)
        {
            var address = IPAddress.Parse(ip);
            server = new KcpServer<KcpSocket>(0);
            server.Run(1);
            server.OpenHeart();
            var remote = new IPEndPoint(address, port);
            link = server.FindOrCreateLink(remote);
            server.soc.SendTo(KcpListener.Heart, remote);
        }
        public int pin;
        public int userId;
        public void FailedConnect()
        {

        }
        public void OpenLog(string ip=null,int port=0)
        {
            Application.logMessageReceived += Log;
            if (ip == null | ip == "")
                logLink = link;
            else {
                var address = IPAddress.Parse(ip);
                logLink= server.FindOrCreateLink(new IPEndPoint(address, port));
            }
        }
        void Log(string condtion, string stack, LogType type)
        {
            LogData log = new LogData();
            log.condition = condtion;
            log.stackTrace = stack;
            log.type = type;
            var str = JsonUtility.ToJson(log);
            logLink.Send(EnvelopeType.String, Encoding.UTF8.GetBytes(str));
        }
        public void CloseLog()
        {
            if (KcpListener.Instance != null)
                KcpListener.Instance.Dispose();
            Application.logMessageReceived -= Log;
            if (link != null)
                link.Dispose();
            link = null;
        }
        public void DispatchMessage()
        {
            try
            {
                if (link != null)
                {
                    if(link._connect)
                        if(!link.connect)
                        {
                            link.connect = true;
                            link.ConnectionOK();
                        }
                    lock (link.datas)
                    {
                        int c = link.datas.Count;
                        for (int i = 0; i < c; i++)
                        {
                            var dat = link.datas.Dequeue();
                            DispatchEx(dat.dat, dat.tag);
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
        float Time;
        void DispatchEx(byte[] data, byte tag)
        {
            byte type = tag;
            switch (type)
            {
                case EnvelopeType.Mate:
                    DispatchMetaData(data);
                    break;
                case EnvelopeType.AesJson:
                    //byte[] dec = AES.Instance.Decrypt(data, 0, data.Length);
                    //var json = Encoding.UTF8.GetString(dec);
                    //DispatchJson(json);
                    break;
                case EnvelopeType.Json:
                    var json = Encoding.UTF8.GetString(data);
                    DispatchJson(json);
                    break;
                case EnvelopeType.AesDataBuffer:
                    //var buff = KcpPack.UnPack(data);
                    //if (buff != null)
                    //    DispatchStream(buff);
                    break;
                case EnvelopeType.DataBuffer:
                    DispatchStream(new DataBuffer(data));
                    break;
                case EnvelopeType.String:
                    json = Encoding.UTF8.GetString(data);
                    DispatchString(json);
                    break;
            }
        }
        void DispatchMetaData(byte[] data)
        {

        }
        void DispatchString(string json)
        {

        }
        void DispatchJson(string json)
        {
            var j = JsonUtility.FromJson<Msg>(json);
            if(j.Error>0)
            {

            }
            else
            {
                switch (j.Type)
                {
                    case MessageType.Def:
                        DefDataControll.Dispatch(j);
                        break;
                }
            }
        }
        void DispatchStream(DataBuffer buffer)
        {
            var fake = buffer.fakeStruct;

            if (fake != null)
            {
                if (fake[Req.Error] > 0)
                {
                    switch (fake[Req.Type])
                    {
                        case MessageType.Def:
                            //DefaultErrorControll.Dispatch(buffer);
                            break;
                        case MessageType.Rpc:
                            //RpcErrorControll.Dispatch(buffer);
                            break;
                        case MessageType.Query:
                            //QueryData.Dispatch(linker, buffer);
                            break;
                    }
                }
                else
                {
                    switch (fake[Req.Type])
                    {
                        case MessageType.Def:
                            DefDataControll.Dispatch(buffer);
                            break;
                        case MessageType.Game:
                            //GameDataControll.Dispatch(buffer);
                            break;
                        case MessageType.Rpc:
                            //RpcDataControll.Dispatch(buffer);
                            break;
                        case MessageType.Query:
                            //QueryData.Dispatch(linker, buffer);
                            break;
                    }
                }
            }
        }
        public void SendJson(int cmd, int type, string json)
        {
            Msg msg = new Msg();
            msg.Cmd = cmd;
            msg.Type = type;
            msg.Args = json;
            string str = JsonUtility.ToJson(msg);
            var dat = Encoding.UTF8.GetBytes(str);
            link.Send(EnvelopeType.Json, dat);
        }
        public void SendAesJson(int cmd,int type,string json)
        {
            Msg msg = new Msg();
            msg.Cmd = cmd;
            msg.Type = type;
            msg.Args = json;
            string str = JsonUtility.ToJson(msg);
            var dat = Encoding.UTF8.GetBytes(str);
            
            //link.Send(dat, EnvelopeType.AesJson);
        }
        public void SendLz4AesJson(int cmd, int type, string json)
        {
            Msg msg = new Msg();
            msg.Cmd = cmd;
            msg.Type = type;
            msg.Args = json;
            string str = JsonUtility.ToJson(msg);
            var dat = Encoding.UTF8.GetBytes(str);

            //link.Send(dat, EnvelopeType.AesJson);
        }
        public void SendStream(DataBuffer db)
        {
            link.Send(EnvelopeType.DataBuffer, db.ToBytes());
        }
        public void SendAesStream(DataBuffer db)
        {
            
        }
        public void SendString(Int32 cmd, Int32 type, string obj)
        {
            
        }
        public void SendNull(Int32 cmd, Int32 type)
        {
            
        }
        public void SendInt(Int32 cmd, Int32 type, int args)
        {
           
        }
        public void SendLong(Int32 cmd, Int32 type, long obj)
        {
            
        }
        public void SendStruct<T>(Int32 cmd, Int32 type, T obj) where T : unmanaged
        {
           
        }
        public void SendObject<T>(Int32 cmd, Int32 type, object obj) where T : class
        {
            
        }
        public void SendMate(byte[] buf)
        {
            link.Send(EnvelopeType.Mate, buf);
        }
        public void Dispose()
        {
            server.CloseHeart();
            server.Dispose();
        }
    }
}
