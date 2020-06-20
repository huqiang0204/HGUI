using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Data
{
    public class LocalSetting
    {
        static INIReader ini;
        static INIReader Setting { get {
                if (ini == null)
                {
                    ini = new INIReader();
                    string filepath = Application.persistentDataPath + "/setting.ini";
                    if (File.Exists(filepath))
                        ini.LoadFromFile(filepath);
                }
                return ini;
            } }
        static bool Changed;
        public static string GetString(string block, string name)
        {
            var sec = Setting.FindSection(block);
            if (sec == null)
                return null;
            return sec.GetValue(name);
        }
        public static int GetInt(string block, string name)
        {
            var sec = Setting.FindSection(block);
            if (sec == null)
                return 0;
            string v = sec.GetValue(name);
            if (v == null)
                return 0;
            int a = 0;
            int.TryParse(v,out a);
            return a;
        }
        public static void SetString(string block, string name,string value)
        {
            var sec = Setting.FindSection(block);
            if (sec == null)
            {
                sec = new INISection();
                sec.name = block;
                Setting.AddSection(sec);
            }
            Changed = true;
            sec.Add(name,value);
        }
        public static void SetInt(string block, string name, int value)
        {
            var sec = Setting.FindSection(block);
            if (sec == null)
            {
                sec = new INISection();
                sec.name = block;
                Setting.AddSection(sec);
            }
            Changed = true;
            sec.Add(name, value.ToString());
        }
        public static void Save()
        {
            if(Changed)
            {
                ini.WriteToFile(Application.persistentDataPath+"/setting.ini");
                Changed = false;
            }
        }
        public static void RemoveBlock(string block)
        {
            if (ini != null)
            {
                if (ini.RemoveSection(block))
                    Changed = true;
            }
        }
    }
}
