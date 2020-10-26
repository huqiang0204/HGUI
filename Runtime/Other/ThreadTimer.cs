using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace huqiang
{
    /// <summary>
    /// 线程计时器
    /// </summary>
    public class ThreadTimer
    {
        bool run = false;
#if UNITY_WSA
        System.Threading.Tasks.Task thread;
#else
         Thread thread;
#endif
        AutoResetEvent auto;
        /// <summary>
        /// 更新委托
        /// </summary>
        public Action<ThreadTimer,Int32> Tick;
        Int32 m_inter;
        public Int32 Interal { set { if (value < 1) value = 1;m_inter = value; } }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="inter">更新间隔,单位毫秒</param>
        public ThreadTimer(Int32 inter = 16)
        {
            Interal = inter;
            auto = new AutoResetEvent(true);
            run = true;
#if UNITY_WSA
            thread = System.Threading.Tasks.Task.Run(Run);
#else
                thread = new Thread(Run);
               thread.Start();
#endif
        }
        void Run()
        {
            long tick = DateTime.Now.Ticks;
            while (run)
            {
                auto.WaitOne(1);
                var l = DateTime.Now.Ticks;
                int t = (int)((l - tick) / 10000);
                if (t > m_inter)
                {
                    tick = l;
                    if (Tick != null)
                        Tick(this, t);
                }
            }
        }
        /// <summary>
        /// 释放线程
        /// </summary>
        public void Dispose()
        {
            run = false;
        }
    }
}
