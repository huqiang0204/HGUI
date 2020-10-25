using huqiang;
using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.Pool;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace huqiang.Data
{
    /// <summary>
    /// 类型信息
    /// </summary>
    public abstract class TypeInfo
    {
        public int Index;
        public Type type;
        public string name;
        public DataLoader loader;
        /// <summary>
        /// 类型比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Compare(object obj)
        {
            return false;
        }
    }
    /// <summary>
    /// unity 组件比较器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ComponentInfo<T> : TypeInfo where T : Component 
    {
        /// <summary>
        /// 构造函数,初始化类型信息
        /// </summary>
        public ComponentInfo()
        {
            type = typeof(T);
            name = type.Name;
        }
        /// <summary>
        /// 比较实例化对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Compare(object obj)
        {
            return obj is T;
        }
    }
    /// <summary>
    /// 反射模型
    /// </summary>
    public class ReflectionModel
    {
        /// <summary>
        /// 类名
        /// </summary>
        public string name;
        /// <summary>
        /// 字段信息
        /// </summary>
        public FieldInfo field;
        public Type FieldType;
        /// <summary>
        /// 反射值
        /// </summary>
        public object Value;
    }
    /// <summary>
    /// 
    /// </summary>
    public class TempReflection
    {
        /// <summary>
        /// 数组的顶部指针
        /// </summary>
        public int Top;
        /// <summary>
        /// 对象的所有字段反射模型
        /// </summary>
        public ReflectionModel[] All;
        /// <summary>
        /// 将类型中的所有公开字段转换成临时反射类
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
        /// <summary>
        /// 将对象中的所有公开字段转换成临时反射类
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <returns></returns>
        public static TempReflection ObjectFields(object obj)
        {
            return ObjectFields(obj.GetType());
        }
    }
    public class GameobjectBuffer
    {
        Transform CycleBuffer;
        /// <summary>
        /// 构造函数,设置回收对象的父物体
        /// </summary>
        /// <param name="buffer">父对象</param>
        public GameobjectBuffer(Transform buffer)
        {
            CycleBuffer = buffer;
        }
        int point;
        TypeInfo[] types = new TypeInfo[63];
        List<ModelBuffer> models = new List<ModelBuffer>();
        Container<InstanceContext> container = new Container<InstanceContext>(4096);
        /// <summary>
        /// 注册一个组件
        /// </summary>
        /// <param name="info"></param>
        public void RegComponent(TypeInfo info)
        {
            if (point >= 63)
                return;
            if (info.loader != null)
                info.loader.gameobjectBuffer = this;
            var name = info.name;
            for (int i = 0; i < point; i++)
                if (types[i].name == name)
                {
                    types[i] = info;
                    return;
                }
            info.Index = point;
            types[point] = info;
            point++;
        }
        /// <summary>
        /// 获取组件的索引
        /// </summary>
        /// <param name="com">组件实例</param>
        /// <returns></returns>
        public Int32 GetTypeIndex(Component com)
        {
            for (int i = 0; i < point; i++)
            {
                if (types[i].Compare(com))
                {
                    return i;
                }
            }
            return 0;
        }
        /// <summary>
        /// 获取组件索引
        /// </summary>
        /// <param name="name">组件名称</param>
        /// <returns></returns>
        public Int32 GetTypeIndex(string name)
        {
            for (int i = 0; i < point; i++)
            {
                if (types[i].name == name)
                {
                    return i;
                }
            }
            return 0;
        }
        /// <summary>
        /// 获取组件索引
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns></returns>
        public Int32 GetTypeIndex<T>()
        {
            var t = typeof(T);
            for (int i = 0; i < point; i++)
            {
                if (types[i].type == t)
                {
                    return i;
                }
            }
            return 0;
        }
        /// <summary>
        /// 获取组件的ID
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public Int64 GetTypeID(Component com)
        {
            for (int i = 0; i < point; i++)
            {
                if (types[i].Compare(com))
                {
                    Int64 a = 1 << i;
                    return a;
                }
            }
            return 1;
        }
        /// <summary>
        /// 获取一组组件的ID
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public Int64 GetTypeID(Component[] com)
        {
            if (com == null)
                return 0;
            Int64 a = 1;
            for (int i = 0; i < com.Length; i++)
            {
                var c = com[i];
                if (c != null)
                    a |= GetTypeID(c);
            }
            return a;
        }
        /// <summary>
        /// 创建一个模型缓存
        /// </summary>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ModelBuffer CreateModelBuffer(Int64 type, Int32 size = 384)
        {
            long t = type;
            List<Type> tmp = new List<Type>();
            for (int i = 1; i < 64; i++)
            {
                t >>= 1;
                if (t > 0)
                {
                    if ((t & 1) > 0)
                    {
                        tmp.Add(types[i].type);
                    }
                }
                else break;
            }
            ModelBuffer model = new ModelBuffer(type, size, tmp.ToArray(), container);
            models.Add(model);
            return model;
        }
        /// <summary>
        /// 通过类型创建一个对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public GameObject CreateNew(Int64 type)
        {
            if (type == 0)
                return null;
            for (int i = 0; i < models.Count; i++)
                if (type == models[i].type)
                    return models[i].CreateObject();
            return CreateModelBuffer(type).CreateObject();
        }
        /// <summary>
        /// 回收游戏对象
        /// </summary>
        /// <param name="game"></param>
        public void RecycleGameObject(GameObject game)
        {
            if (game == null)
                return;
            int id = game.GetInstanceID();
            InstanceContext ins = container.Find((o) => { return o.Id == id; });
            if (ins != null)
                ins.buffer.ReCycle(game);
            var p = game.transform;
            for (int i = p.childCount - 1; i >= 0; i--)
                RecycleGameObject(p.GetChild(i).gameObject);
            if (ins != null)
            { 
                p.SetParent(CycleBuffer);
            }
            else
                GameObject.Destroy(game);
        }
        /// <summary>
        /// 回收对象的子物体
        /// </summary>
        /// <param name="game"></param>
        public void RecycleChild(GameObject game)
        {
            if (game == null)
                return;
            var trans = game.transform;
            int c = trans.childCount - 1;
            for (int i = c; i >= 0; i--)
            {
                RecycleGameObject(trans.GetChild(i).gameObject);
            }
        }
        /// <summary>
        /// 回收除开相应名称意外的对象的子物体
        /// </summary>
        /// <param name="game">游戏对象</param>
        /// <param name="keep">要保留子对象的名称数组</param>
        public void RecycleChild(GameObject game, string[] keep)
        {
            if (game == null)
                return;
            var trans = game.transform;
            int c = trans.childCount - 1;
            for (int i = c; i >= 0; i--)
            {
                var son = trans.GetChild(i);
                if (!keep.Contains(son.name))
                    RecycleGameObject(trans.GetChild(i).gameObject);
            }
        }
        /// <summary>
        /// 查询组件的数据载入器
        /// </summary>
        /// <param name="com">组件实例</param>
        /// <returns></returns>
        public DataLoader FindDataLoader(Component com)
        {
            for(int i=0;i<types.Length;i++)
            {
                if (types[i].Compare(com))
                    return types[i].loader;
            }
            return null;
        }
        /// <summary>
        /// 获取数据载入器
        /// </summary>
        /// <param name="Index">索引id</param>
        /// <returns></returns>
        public DataLoader GetDataLoader(int Index)
        {
            return types[Index].loader;
        }
        /// <summary>
        /// 查询transform的子物体
        /// </summary>
        /// <param name="fake"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public unsafe FakeStruct FindChild(FakeStruct fake, string childName)
        {
            var data = (TransfromData*)fake.ip;
            var buff = fake.buffer;
            Int16[] chi = fake.buffer.GetData(data->child) as Int16[];
            if (chi != null)
                for (int i = 0; i < chi.Length; i++)
                {
                    var fs = buff.GetData(chi[i]) as FakeStruct;
                    if (fs != null)
                    {
                        var cd = (TransfromData*)fs.ip;
                        string name = buff.GetData(cd->name) as string;
                        if (name == childName)
                            return fs;
                    }
                }
            return null;
        }
        /// <summary>
        /// 克隆一个预制体对象
        /// </summary>
        /// <param name="fake"></param>
        public GameObject Clone(FakeStruct fake)
        {
            if (fake == null)
                return null;
            long id = fake.GetInt64(0);
            var go = CreateNew(id);
            types[0].loader.LoadToObject(fake,go.transform,null);
            return go;
        }
        /// <summary>
        /// 克隆某个游戏对象
        /// </summary>
        /// <param name="fake">假结构体数据</param>
        /// <param name="initializer">初始化器</param>
        /// <returns></returns>
        public GameObject Clone(FakeStruct fake, Initializer initializer)
        {
            if (fake == null)
                return null;
            long id = fake.GetInt64(0);
            var go = CreateNew(id);
            types[0].loader.LoadToObject(fake, go.transform,initializer);
            if (initializer != null)
                initializer.Done();
            return go;
        }
    }
}
