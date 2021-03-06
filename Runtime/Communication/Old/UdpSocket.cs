﻿using huqiang.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace huqiang
{
    public class UdpSocket
    {
        Socket soc;
        Thread thread;
        TcpEnvelope envelope;
        IPEndPoint endPoint;
        /// <summary>
        /// 是否开启封包功能,默认关闭
        /// </summary>
        public bool Packaging = false;
        bool running;
        bool auto;
        QueueBuffer<SocData> queue;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="port">端口</param>
        /// <param name="remote">远程ip地址</param>
        /// <param name="subThread">是否运行子线程派发消息</param>
        /// <param name="type">封包类型</param>
        /// <param name="es">封包缓存大小</param>
        public UdpSocket(int port, IPEndPoint remote, bool subThread = true, PackType type = PackType.Total, int es = 262144)
        {
            endPoint = remote;
            //Links = new Linker[thread * 1024];
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);
            soc = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); //new UdpClient(_port);//new IPEndPoint(IPAddress.Parse(ip),
            soc.Bind(ip);
            soc.ReceiveTimeout = 1000;

            if (type != PackType.None)
            {
                Packaging = true;
                envelope = new TcpEnvelope(es);
                envelope.type = type;
            }
            running = true;
            auto = subThread;
            if (thread == null)
            {
                thread = new Thread(Run);
                thread.Start();
            }
            queue = new QueueBuffer<SocData>();
        }
    
        void Run()
        {
            byte[] buffer = new byte[65536];
            while (running)
            {
                try
                {
                    EndPoint end = endPoint;
                    int len = soc.ReceiveFrom(buffer, ref end);//接收数据报
                    if (len > 0)
                    {
                        byte[] dat = new byte[len];
                        for (int i = 0; i < len; i++)
                            dat[i] = buffer[i];
                        if (Packaging)
                        {
                            var data = envelope.Unpack(dat, len);
                            for (int i = 0; i < data.Count; i++)
                            {
                                var item = data[i];
                                EnvelopeCallback(item.data, item.type);
                            }
                        }
                        else
                        {
                            EnvelopeCallback(dat, 0);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                }
            }
        }
        void EnvelopeCallback(byte[] data,byte tag)
        {
            if (auto)
            {
                if (MainDispatch != null)
                    MainDispatch(data, tag, endPoint);
            }
            else
            {
                SocData soc = new SocData();
                soc.data = data;
                soc.tag = tag;
                soc.obj = endPoint;
                queue.Enqueue(soc);
            }
        }
        /// <summary>
        /// 如果只能主线程派发则使用此委托
        /// </summary>
        public Action<byte[], byte, IPEndPoint> MainDispatch;
        /// <summary>
        /// 派发接收到的消息
        /// </summary>
        public void Dispatch()
        {
            if (queue != null)
            {
                int c = queue.Count;
                SocData soc;
                for (int i = 0; i < c; i++)
                {
                    soc = queue.Dequeue();
                    if (soc != null)
                        if (MainDispatch != null)
                            MainDispatch(soc.data, soc.tag, soc.obj as IPEndPoint);
                }
            }
        }
        /// <summary>
        /// 关Socket和线程
        /// </summary>
        public void Close()
        {
            soc.Close();
            running = false;
        }
        /// <summary>
        /// 向远程发送一条消息
        /// </summary>
        /// <param name="dat">数据</param>
        /// <param name="point">远程地址</param>
        /// <param name="tag">数据类型</param>
        /// <returns></returns>
        public bool Send(byte[] dat, IPEndPoint point, byte tag)
        {
            try
            {
                if (Packaging)
                {
                    var buf = envelope.Pack(dat, tag);
                    if (buf != null)
                        for (int i = 0; i < buf.Length; i++)
                            soc.SendTo(buf[i], point);
                }
                else soc.SendTo(dat, point);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="dat">数据</param>
        /// <param name="port">端口</param>
        /// <param name="tag">数据类型</param>
        public void Broadcast(byte[] dat, int port,byte tag)
        {
            var ip = new IPEndPoint(IPAddress.Broadcast, port);
            if (Packaging)
            {
                var buf = envelope.Pack(dat, tag);
                if (buf != null)
                    for (int i = 0; i < buf.Length; i++)
                        soc.SendTo(buf[i],  ip);
            }
            else soc.SendTo(dat, ip);
            endPoint.Address = IPAddress.Any;
        }
        /// <summary>
        /// 远程地址重定向
        /// </summary>
        /// <param name="address">远程地址</param>
        public void Redirect(IPAddress address)
        {
            endPoint.Address = address;
        }
    }
}