using huqiang;
using System;
using System.Net.Sockets;
using System.Text;
using System.Net;

namespace huqiang
{
    /// <summary>
    /// 客户端连接
    /// </summary>
    public class TcpLink:NetworkLink
    {
        //PlayerInfo playerInfo;
        TcpEnvelope envelope=new TcpEnvelope();
        internal Socket Link;
        byte[] buff;
        /// <summary>
        /// 设置Socket
        /// </summary>
        /// <param name="soc">Socket</param>
        /// <param name="end">远程ip地址</param>
        /// <param name="pack">封包类型</param>
        /// <param name="buffsize">数据缓冲区大小</param>
        public void SetSocket(Socket soc,IPEndPoint end,PackType pack=PackType.Part,int buffsize=4096)
        {
            Link = soc;
            envelope.type = pack;
            endpPoint = end;
            addr = end.Address;
            buff = new byte[buffsize];
            var buf = addr.GetAddressBytes();
            unsafe
            {
                fixed (byte* bp = &buf[0])
                    ip = *(int*)bp;
            }
            port = end.Port;
        }
        /// <summary>
        /// 设置封包类型
        /// </summary>
        /// <param name="pack">封包类型</param>
        public void SetPackType(PackType pack)
        {
            envelope.type = pack;
        }
        //玩家登录ip
        public IPAddress addr;
        /// <summary>
        /// 发送一条消息
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="type">数据类型</param>
        /// <returns></returns>
        public int Send(byte[] data,byte type = 0)
        {
            try
            {
                if (Link != null)
                    lock (Link)
                        if (Link.Connected)
                        {
                            var ss = envelope.Pack(data,type);
                            for (int i = 0; i < ss.Length; i++)
                            {
                                Link.Send(ss[i]);
                            }
                            return 1;
                        }
                        else return -1;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return -1;
            }
            return 0;
        }
        /// <summary>
        /// 发送一组数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public int Send(byte[][] data)
        {
            try
            {
                if (Link != null)
                    lock (Link)
                        if (Link.Connected)
                        {
                            for (int i = 0; i < data.Length; i++)
                                Link.Send(data[i]);
                            return 1;
                        }
                        else return -1;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return -1;
            }
            return 0;
        }
        /// <summary>
        /// 发送一条字符串数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public int Send(string data)
        {
            return Send(Encoding.UTF8.GetBytes(data));
        }
        ~TcpLink()
        {
            if (Link != null)
                lock (Link)
                    Link.Close();
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            if (Link != null)
                lock (Link)
                    Link.Close();
        }
        /// <summary>
        /// 处理接收到的消息
        /// </summary>
        /// <param name="time">当前时间</param>
        public override void Recive(long time)
        {
            try
            {
                int len = Link.Receive(buff, SocketFlags.Peek);
                if (len > 0)
                {
                    len = Link.Receive(buff);
                    var list= envelope.Unpack(buff,len);
                    try
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            var dat = list[i];
                            Dispatch(dat.data, dat.type);
                        }
                    }catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// 派发解析好的数据
        /// </summary>
        /// <param name="dat">数据</param>
        /// <param name="tag">数据类型</param>
        public virtual void Dispatch( byte[] dat, byte tag)
        {
        }
    }
}
