using huqiang.Core.HGUI;
using huqiang.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UIModel
{
    public class UIBase
    {
        /// <summary>
        /// 当前语言
        /// </summary>
        internal static string LanName;
        /// <summary>
        /// 语言数据
        /// </summary>
        public static INIReader Lan = new INIReader();
        /// <summary>
        /// 设置当前语言
        /// </summary>
        /// <param name="name">语言名称</param>
        /// <param name="iniData">语言数据</param>
        public static void SetLanguage(string name,byte[] iniData)
        {
            LanName = name;
            Lan.LoadData(iniData);
        }
        static int point;
        /// <summary>
        /// ui缓存
        /// </summary>
        static UIBase[] buff = new UIBase[1024];
        /// <summary>
        /// 获取某个UI
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public static T GetUI<T>() where T : UIBase
        {
            for (int i = 0; i < point; i++)
                if (buff[i] is T)
                    return buff[i] as T;
            return null;
        }
        /// <summary>
        /// 向某个UI类投递消息
        /// </summary>
        /// <typeparam name="T">ui类型</typeparam>
        /// <param name="cmd">指令</param>
        /// <param name="dat">数据</param>
        public static void PostMsg<T>(string cmd, object dat) where T : UIBase
        {
            for (int i = 0; i < point; i++)
                if (buff[i] is T)
                {
                    buff[i].Cmd(cmd, dat);
                    return;
                }
        }
        /// <summary>
        /// 向某个UI类投递消息
        /// </summary>
        /// <typeparam name="T">ui类型</typeparam>
        /// <param name="cmd">指令</param>
        /// <param name="dat">数据</param>
        public static void PostMsg<T>(int cmd, object dat) where T : UIBase
        {
            for (int i = 0; i < point; i++)
                if (buff[i] is T)
                {
                    buff[i].Cmd(cmd, dat);
                    return;
                }
        }
        /// <summary>
        /// 获取某个类型的ui列表
        /// </summary>
        /// <typeparam name="T">ui类型</typeparam>
        /// <returns></returns>
        public static List<T> GetUIs<T>() where T : UIBase
        {
            List<T> tmp = new List<T>();
            for (int i = 0; i < point; i++)
                if (buff[i] is T)
                    tmp.Add(buff[i] as T);
            return tmp;
        }
        /// <summary>
        /// 清除所有UI
        /// </summary>
        public static void ClearUI()
        {
            for (int i = 0; i < point; i++)
                buff[i] = null;
            point = 0;
        }
        int Index;
        /// <summary>
        /// 构造函数
        /// </summary>
        public UIBase()
        {
            Index = point;
            buff[point] = this;
            point++;
        }
        /// <summary>
        /// 联系上下文
        /// </summary>
        public object DataContext;
        /// <summary>
        /// 父坐标变换
        /// </summary>
        public Transform Parent { get; protected set; }
        /// <summary>
        /// 主游戏对象
        /// </summary>
        public GameObject Main { get; protected set; }
        /// <summary>
        /// UI模型
        /// </summary>
        public FakeStruct model { get; protected set; }
        /// <summary>
        /// 父UI对象
        /// </summary>
        protected UIBase UIParent;
        /// <summary>
        /// 对象转换到反射列表
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static TempReflection ObjectFields(Type type)
        {
            var fs = type.GetFields();
            TempReflection temp = new TempReflection();
            temp.Top = fs.Length;
            ReflectionModel[] reflections = new ReflectionModel[temp.Top];
            for (int i = 0; i < fs.Length; i++)
            {
                ReflectionModel r = new ReflectionModel();
                r.field = fs[i];
                r.FieldType = fs[i].FieldType;
                r.name = fs[i].Name;
                reflections[i] = r;
            }
            temp.All = reflections;
            return temp;
        }
        string uiName;
        /// <summary>
        /// UI初始化器实例
        /// </summary>
        protected UIInitializer uiInitializer;
        /// <summary>
        /// 载入UI模型并实例化
        /// </summary>
        /// <typeparam name="T">UI模型</typeparam>
        /// <param name="asset">资源包名</param>
        /// <param name="name">UI名</param>
        /// <returns></returns>
        public T LoadUI<T>(string asset, string name) where T : class, new()
        {
            uiName = name;
            model = HGUIManager.FindModelAndSetAssets(asset, name);
            T t = new T();
            uiInitializer = new UIInitializer(ObjectFields(typeof(T)));
            uiInitializer.Reset(t);
            Main = HGUIManager.GameBuffer.Clone(model, uiInitializer);
            var trans = Main.transform;
            trans.SetParent(Parent);
            trans.localPosition = Vector3.zero;
            trans.localScale = Vector3.one;
            trans.localRotation = Quaternion.identity;
            return t;
        }
        /// <summary>
        /// 初始化UI
        /// </summary>
        /// <param name="parent">父坐标变换</param>
        /// <param name="ui">父UI</param>
        /// <param name="obj">传递的数据</param>
        public virtual void Initial(Transform parent, UIBase ui, object obj = null)
        {
            DataContext = obj;
            UIParent = ui;
            Parent = parent;
            if (parent != null)
                if (Main != null)
                    Main.transform.SetParent(parent);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            if (Main != null)
            {
                HGUIManager.GameBuffer.RecycleGameObject(Main);
            }
            point--;
            if (buff[point] != null)
                buff[point].Index = Index;
            buff[Index] = buff[point];
            buff[point] = null;
        }
        public virtual object CollectionData()
        {
            return null;
        }
        /// <summary>
        /// 接受指令
        /// </summary>
        /// <param name="msg">指令</param>
        /// <param name="dat">数据</param>
        public virtual void Cmd(Msg msg, object dat)
        {
        }
        /// <summary>
        /// 接受指令
        /// </summary>
        /// <param name="cmd">指令</param>
        /// <param name="dat">数据</param>
        public virtual void Cmd(int cmd, object dat)
        {
        }
        /// <summary>
        /// 接受指令
        /// </summary>
        /// <param name="cmd">指令</param>
        /// <param name="dat">数据</param>
        public virtual void Cmd(string cmd, object dat)
        {
        }
        /// <summary>
        /// 接受指令
        /// </summary>
        /// <param name="dat">数据</param>
        public virtual void Cmd(DataBuffer dat)
        {
        }
        /// <summary>
        /// 更新布局尺寸
        /// </summary>
        public virtual void ReSize()
        {
            if (Main != null)
            {
                var ele = Main.GetComponent<UIElement>();
                if (ele != null)
                    UIElement.Resize(ele);
            }
        }
        public virtual void Update(float time)
        {
        }
        /// <summary>
        /// 当前UI的语言节点
        /// </summary>
        protected INISection LanSection;
        /// <summary>
        /// 当前语言
        /// </summary>
        internal string CurLan;
        /// <summary>
        /// 更换语言
        /// </summary>
        /// <returns></returns>
        public virtual bool ChangeLanguage()
        {
            if (CurLan == LanName)
                return false;
            if (Lan == null)
                return false;
            CurLan = LanName;
            if (uiName != null)
            {
               LanSection = Lan.FindSection(uiName);
                if (LanSection != null)
                {
                    if (uiInitializer != null)
                    {
                        uiInitializer.ChangeLanguage(LanSection);
                    }
                }
            }
            return true;
        }
    }
}

