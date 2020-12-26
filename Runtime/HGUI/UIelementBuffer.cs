using huqiang.Core.UIData;
using huqiang.Data;
using huqiang.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public class UIelementBuffer
    {
        /// <summary>
        /// 类型信息
        /// </summary>
        public class UITypeInfo
        {
            public string typeName;
            public virtual UIElement CreateNew() { return new UIElement(); }
            public UIDataLoader loader;
            public List<UIElement> buffer = new List<UIElement>();
        }
        public class UITypeInfo<T> : UITypeInfo where T : UIElement, new()
        {
            public override UIElement CreateNew()
            {
                return new T();
            }
        }
        int point = 0;
        public int TypeSize { get => point; }
        UITypeInfo[] types = new UITypeInfo[63];
        /// <summary>
        /// 注册一个组件
        /// </summary>
        /// <param name="info"></param>
        public void RegDataLoader<T>(UIDataLoader loader)where T : UIElement, new()
        {
            if (point >= 63)
                return;
            loader.uiBuffer = this;
            string typeName = typeof(T).Name;
            for (int i = 0; i < point; i++)
                if (types[i].typeName == typeName)
                {
                    types[i].loader = loader;
                    return;
                }
            UITypeInfo info = new UITypeInfo<T>();
            info.typeName = typeName;
            info.loader = loader;
            types[point] = info;
            point++;
        }
        /// <summary>
        /// 通过类型创建一个对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public UIElement CreateNew(string type)
        {
            for (int i = 0; i < point; i++)
            {
                var item = types[i];
                if (type == item.typeName)
                {
                    int c = item.buffer.Count;
                    if (c > 0)
                    {
                        var ui = item.buffer[c - 1];
                        item.buffer.RemoveAt(c - 1);
                        return ui;
                    }
                    return item.CreateNew();
                }
            }
            return null;
        }
        /// <summary>
        /// 回收游戏对象
        /// </summary>
        /// <param name="ui"></param>
        public void RecycleUI(UIElement ui)
        {
            if (ui == null)
                return;
            Recycle(ui);
        }
        void Recycle(UIElement game)
        {
            game.Clear();
            game.SetParent(null);
            var tn = game.TypeName;
            for (int i = 0; i < point; i++)
            {
                if (types[i].typeName == tn)
                {
                    types[i].buffer.Add(game);
                    break;
                }
            }
            int c = game.child.Count;
            for (int i = c - 1; i >= 0; i--)
            {
                Recycle(game.child[i]);
            }
        }
        /// <summary>
        /// 回收对象的子物体
        /// </summary>
        /// <param name="ui"></param>
        public void RecycleChild(UIElement ui)
        {
            if (ui == null)
                return;
            int c = ui.childCount - 1;
            for (int i = c; i >= 0; i--)
            {
                RecycleUI(ui.GetChild(i));
            }
        }
        /// <summary>
        /// 回收除开相应名称意外的对象的子物体
        /// </summary>
        /// <param name="ui">游戏对象</param>
        /// <param name="keep">要保留子对象的名称数组</param>
        public void RecycleChild(UIElement ui, string[] keep)
        {
            if (ui == null)
                return;
            int c = ui.childCount - 1;
            for (int i = c; i >= 0; i--)
            {
                var son = ui.GetChild(i);
                if (!keep.Contains(son.name))
                    RecycleUI(son);
            }
        }
        /// <summary>
        /// 查询数据载入器
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public UIDataLoader FindDataLoader(string typeName)
        {
            for (int i = 0; i < point; i++)
            {
                if (types[i].typeName == typeName)
                    return types[i].loader;
            }
            return null;
        }
        /// <summary>
        /// 查询transform的子物体
        /// </summary>
        /// <param name="fake"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public unsafe FakeStruct FindChild(FakeStruct fake, string childName)
        {
            var data = (UIElementData*)fake.ip;
            var buff = fake.buffer;
            Int16[] chi = fake.buffer.GetData(data->child) as Int16[];
            if (chi != null)
                for (int i = 0; i < chi.Length; i++)
                {
                    var fs = buff.GetData(chi[i]) as FakeStruct;
                    if (fs != null)
                    {
                        var cd = (UIElementData*)fs.ip;
                        string name = buff.GetData(cd->name) as string;
                        if (name == childName)
                            return fs;
                    }
                }
            return null;
        }
        /// <summary>
        /// 克隆某个游戏对象
        /// </summary>
        /// <param name="fake">假结构体数据</param>
        /// <param name="initializer">初始化器</param>
        /// <returns></returns>
        public UIElement Clone(FakeStruct fake, UIInitializer initializer)
        {
            string type = fake.GetData<string>(0);
            for (int i = 0; i < point; i++)
            {
                var item = types[i];
                if (item.typeName == type)
                {
                    int c = item.buffer.Count;
                    UIElement ui;
                    if (c > 0)
                    {
                        ui = item.buffer[c - 1];
                        item.buffer.RemoveAt(c - 1);
                    }
                    else ui = item.CreateNew();
                    item.loader.LoadUI(ui, fake, initializer);
                    return ui;
                }
            }
#if UNITY_EDITOR
            Debug.LogError("不存在或未注册的UI类型:" + type);
#endif
            return null;
        }
        string curTables;
        public void SetTables(FakeStruct[] fakes, string name)
        {
            if (curTables == name)
                return;
            for (int i = 0; i < helpers.Count; i++)
            {
                SetOriTable(fakes, helpers[i]);
            }
        }
        void SetOriTable(FakeStruct[] fakes, FakeStructHelper helper)
        {
            string hn = helper.Name;
            int c = fakes.Length;
            for (int j = 0; j < c; j++)
            {
                var fs = fakes[j];
                string name = fs.GetData<string>(0);
                if (name == hn)
                {
                    helper.SetOriginModel(fs);
                    return;
                }
            }
            helper.SetOriginModel(null);
        }
        public List<FakeStructHelper> helpers = new List<FakeStructHelper>();
        public FakeStructHelper RegFakeStructHelper<T>() where T : unmanaged
        {
            FakeStructHelper helper = new FakeStructHelper();
            helper.SetTargetModel<T>();
            helpers.Add(helper);
            return helper;
        }
    }
}
