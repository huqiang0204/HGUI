using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIModel;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Net
{
    public class DefDataControll
    {
        public const int Update = 0;
        public static void Dispatch(DataBuffer buffer)
        {
            var fake = buffer.fakeStruct;
            switch (fake[Req.Cmd])
            {
                case Update:
                    UpdateUI(buffer);
                    break;
                default:
                    UIPage.CurrentPage.Cmd(buffer);
                    break;
            }
        }
        static void UpdateUI(DataBuffer buffer)
        {
            var fake = buffer.fakeStruct;
            var fs = fake.GetData<FakeStruct>(Req.Args);
            if(fs!=null)
            {
                var ui = fs.GetData<byte[]>(0);
                var dll= fs.GetData<byte[]>(1);
                string data = fs.GetData<string>(2);
                string level = fs.GetData<string>(3);
            }
        }
    }
}
