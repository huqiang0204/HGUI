using huqiang.Core.HGUI;
using huqiang.Core.UIData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[HGUIEditor(typeof(UIElement))]
public class UIEditor
{
    public UIElement Target;
    public virtual void OnEnable()
    {

    }
    public virtual void OnDisable()
    {

    }
    List<Type> list = new List<Type>();
    public virtual void OnInspectorGUI()
    {
        if (Target != null)
        {
            list.Clear();
            var type = Target.GetType();
            var us = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance| BindingFlags.Public);
            if(us!=null)
            {
                if(us.Length>0)
                {
                  
                    GUIStyle style = new GUIStyle();
                    style.fontSize = 14;
                    style.fontStyle = FontStyle.Bold;
                    style.normal.textColor = Color.white;
                    string name = "UIElement";
                    int e = us.Length;
                    for (int c = e - 1; c >= 0; c--)
                    {
                        if (us[c].DeclaringType.Name != name)
                        {
                            EditorGUILayout.LabelField(name,style);
                            name = us[c].DeclaringType.Name;
                            for (int j = c + 1; j < e; j++)
                            {
                                LayoutField(us[j], Target);
                            }
                            e = c + 1;
                        }
                    }
                    
                    EditorGUILayout.LabelField(us[0].DeclaringType.Name,style);
                    for (int j = 0; j < e; j++)
                    {
                        LayoutField(us[j], Target);
                    }
                }
            }
            if (GUI.changed)
            {
                var grap = Target as HGraphics;
                if (grap != null)
                    grap.m_dirty = true;
            }
        }
    }
    protected void LayoutField(FieldInfo field, object ins)
    {
        var hide = field.GetCustomAttribute<HideInInspector>();
        if (hide != null)
            return;
        if (!field.IsPublic)
            if (field.GetCustomAttribute<SerializeField>() == null)
                return;
        Type type = field.FieldType;
        string name = field.Name;
        
        if (type.IsArray)
        {
            var son = type.GetElementType();
        }
        else if (type.IsClass)
        {
            if (type.Name == "String")
            {
                string v = field.GetValue(ins) as string;
                var area = field.GetCustomAttribute<TextAreaAttribute>();
                if(area!=null)
                {
                    EditorGUILayout.LabelField(field.Name);
                    string str = EditorGUILayout.TextArea(v, GUILayout.MinHeight(area.minLines*14.2f), GUILayout.MaxHeight(area.maxLines*14.2f));
                    if (v != str)
                    {
                        field.SetValue(ins, str);
                    }
                }
                else
                {
                    string str = EditorGUILayout.TextField(name, v);
                    if (v != str)
                    {
                        field.SetValue(ins, str);
                    }
                }
            }
            else if (type.IsGenericType)//泛型类
            {
                var args = type.GetGenericArguments();
                if(args.Length>0)
                {
                    var son = args[0];
                }
            }
            else
            {
                UnityEngine.Object v = field.GetValue(ins) as UnityEngine.Object;
                UnityEngine.Object o = EditorGUILayout.ObjectField(field.Name,v, field.FieldType, true);
                if (o != v)
                {
                    field.SetValue(ins, o);
                }
            }
        }
        else if (type.IsEnum)
        {
            Enum v = field.GetValue(ins) as Enum;
            var o = EditorGUILayout.EnumPopup(field.Name, v);
            if (o != v)
            {
                field.SetValue(ins, o);
            }
        }
        else if (type.IsValueType)
        {
            LayoutValueField(field,ins);
        }
    }
    protected void LayoutValueField(FieldInfo field, object ins)
    {
        string type = field.FieldType.Name;
        switch (type)
        {
            case "Int16":
            case "UInt16":
            case "Char":
                {
                    int v = (Int16)field.GetValue(ins);
                    int o = EditorGUILayout.IntField(field.Name, v);
                    if (o != v)
                    {
                        field.SetValue(ins, (Int16)o);
                    }
                }
                break;
            case "Int32":
            case "UInt32":
                {
                    int v = (Int32)field.GetValue(ins);
                    int o = EditorGUILayout.IntField(field.Name, v);
                    if (o != v)
                    {
                        field.SetValue(ins, o);
                    }
                }
                break;
            case "Single":
                {
                    float v = (float)field.GetValue(ins);
                    float o = EditorGUILayout.FloatField(field.Name, v);
                    if (o != v)
                    {
                        field.SetValue(ins, o);
                    }
                }
                break;
            case "Int64":
            case "UInt64":
                {
                    Int64 v = (Int64)field.GetValue(ins);
                    Int64 o = EditorGUILayout.LongField(field.Name, v);
                    if (o != v)
                    {
                        field.SetValue(ins, o);
                    }
                }
                break;
            case "Double":
                {
                    double v = (double)field.GetValue(ins);
                    double o = EditorGUILayout.DoubleField(field.Name, v);
                    if (o != v)
                    {
                        field.SetValue(ins, o);
                    }
                }
                break;
            case "Boolean":
                {
                    bool v = (bool)field.GetValue(ins);
                    bool o = EditorGUILayout.Toggle(field.Name, v);
                    if (o != v)
                    {
                        field.SetValue(ins, o);
                    }
                }
                break;
            case "Vector2":
                {
                    Vector2 v = (Vector2)field.GetValue(ins);
                    Vector2 o = EditorGUILayout.Vector2Field(field.Name, v);
                    if (o != v)
                    {
                        field.SetValue(ins, o);
                    }
                }
                break;
            case "Vector3":
                {
                    Vector3 v = (Vector3)field.GetValue(ins);
                    Vector3 o = EditorGUILayout.Vector3Field(field.Name, v);
                    if (o != v)
                    {
                        field.SetValue(ins, o);
                    }
                }
                break;
            case "Vector4":
                {
                    Vector4 v = (Vector4)field.GetValue(ins);
                    Vector4 o = EditorGUILayout.Vector4Field(field.Name, v);
                    if (o != v)
                    {
                        field.SetValue(ins, o);
                    }
                }
                break;
            case "Color":
                {
                    Color v = (Color)field.GetValue(ins);
                    Color o = EditorGUILayout.ColorField(field.Name, v);
                    if (o != v)
                    {
                        field.SetValue(ins, o);
                    }
                }
                break;
            case "Color32":
                {
                    Color32 v = (Color32)field.GetValue(ins);
                    Color32 o = EditorGUILayout.ColorField(field.Name, v);
                    if (o.a != v.a|o.r!=v.r|o.g != v.g | o.b != v.b)
                    {
                        field.SetValue(ins, o);
                    }
                }
                break;
            case "Quaternion":
                {
                    Quaternion v = (Quaternion)field.GetValue(ins);
                    Vector3 a = v.eulerAngles;
                    Vector3 o = EditorGUILayout.Vector3Field(field.Name, a);
                    if (o != a)
                    {
                        field.SetValue(ins, Quaternion.Euler(o));
                    }
                }
                break;
            default:
                EditorGUILayout.LabelField(field.Name);
                var fs = field.FieldType.GetFields(BindingFlags.Instance|BindingFlags.Public);
                var obj = field.GetValue(ins);
                for (int i = 0; i < fs.Length; i++)
                {
                    LayoutField(fs[i], obj);
                }
                field.SetValue(ins, obj);
                break;
        }
    }
    protected void LayoutArrayField()
    {
        //EditorGUILayout.Foldout();
    }
    public virtual void DrawHelper()
    {
        
    }
    public void DrawEvent()
    {
        if (Target != null)
        {
            switch (Target.eventType)
            {
                case HEventType.UserEvent:
                    break;
                case HEventType.TextInput:
                    break;
                case HEventType.GestureEvent:
                    break;
            }
        }
    }
    public void DrawComposite()
    {
        if (Target != null)
        {
            
        }
    }
}
