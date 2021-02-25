using huqiang;
using huqiang.Core.HGUI;
using huqiang.Core.UIData;
using huqiang.Data;
using System;
using UnityEngine;

namespace huqiang.UIEvent
{
    public enum EntityType
    {
        Default,
        Circle,

    }
    public class UserEvent
    {
    
        internal int Frame;
        /// <summary>
        /// 单击事件触发时间间隔 默认1.8S
        /// </summary>
        public static long ClickTime = 1800000;
        /// <summary>
        /// 当按压和弹起的距离平方小于此距离时触发单击事件,否则判定为拖拽事件,默认为20:(20*20=400)
        /// </summary>
        public static float ClickArea = 400;
        /// <summary>
        /// 派发用户事件
        /// </summary>
        /// <param name="action"></param>
        /// <param name="pipeLine"></param>
        internal static bool DispatchEvent(UserAction action, HGUIElement[] pipeLine)
        {
            HGUIElement root = pipeLine[0];
            //float s =UISystem.PhysicalScale;
            if (root.script != null)
            {
                int c = root.childCount;
                for (int i = c; i > 0; i--)
                {
                    try
                    {
                        if (DispatchEvent(pipeLine, i, action))
                            return true;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.StackTrace);
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 派发用户事件
        /// </summary>
        /// <param name="pipeLine">所有UI</param>
        /// <param name="index"></param>
        /// <param name="pos">父级位置</param>
        /// <param name="scale">父级大小</param>
        /// <param name="quate">父级旋转</param>
        /// <param name="action">用户操作指令</param>
        /// <returns></returns>
        static bool DispatchEvent(HGUIElement[] pipeLine, int index,UserAction action)
        {
            if (!pipeLine[index].active)
                return false;
            int pi = pipeLine[index].parentIndex;
            Vector3 o = pipeLine[index].Position;
            Vector3 scale = pipeLine[index].Scale;
            Quaternion q = pipeLine[index].Rotation;

            var script = pipeLine[index].script;
            if (script != null)
            {
                var ue = script.userEvent;
                if (ue == null)
                {
                    int c = pipeLine[index].childCount;
                    if(c > 0)
                    {
                        int os = pipeLine[index].childOffset + c;
                        for (int i = 0; i < c; i++)
                        {
                            os--;
                            if (DispatchEvent(pipeLine, os,action))
                            {
                                return true;
                            }
                        }
                    }
                }
                else if (ue.forbid)
                {
                    int c = pipeLine[index].childCount;
                    if (c > 0)
                    {
                        int os = pipeLine[index].childOffset + c;
                        for (int i = 0; i < c; i++)
                        {
                            os--;
                            if (DispatchEvent(pipeLine, os, action))
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    ue.pgs = pipeLine[pi].Scale;
                    ue.GlobalScale = scale;
                    ue.GlobalPosition = o;
                    ue.GlobalRotation = q;
                    bool inside = false;
                    if (ue.collider != null)
                       inside = ue.collider.InThere(script, ue, action.CanPosition);
                    if (inside)
                    {
                        action.CurrentEntry.Add(ue);
                        int c = pipeLine[index].childCount;
                        if (c > 0)
                        {
                            int os = pipeLine[index].childOffset + c;
                            for (int i = 0; i < c; i++)
                            {
                                os--;
                                if (DispatchEvent(pipeLine, os, action))
                                {
                                    if (ue.ForceEvent)
                                    {
                                        if (!ue.forbid)
                                            break;
                                    }
                                    return true;
                                }
                            }
                        }
                        if (action.IsLeftButtonDown | action.IsRightButtonDown | action.IsMiddleButtonDown)
                        {
                            ue.OnMouseDown(action);
                        }
                        else if (action.IsLeftButtonUp | action.IsRightButtonUp | action.IsMiddleButtonUp)
                        {
                            ue.OnMouseUp(action);
                        }
                        else
                        {
                            ue.OnMouseMove(action);
                        }
                        if (ue.Penetrate)
                            return false;
                        return true;
                    }
                    else if (!ue.CutRect)
                    {
                        int c = pipeLine[index].childCount;
                        if (c > 0)
                        {
                            int os = pipeLine[index].childOffset + c;
                            for (int i = 0; i < c; i++)
                            {
                                os--;
                                if (DispatchEvent(pipeLine, os, action))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                int c = pipeLine[index].childCount;
                if (c > 0)
                {
                    int os = pipeLine[index].childOffset + c;
                    for (int i = 0; i < c; i++)
                    {
                        os--;
                        if (DispatchEvent(pipeLine, os, action))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        Vector2 mVelocity;
        /// <summary>
        /// 父物体的全局缩放
        /// </summary>
        protected Vector3 pgs = Vector3.one;
        /// <summary>
        /// 全局缩放
        /// </summary>
        public Vector3 GlobalScale = Vector3.one;
        /// <summary>
        /// 全局坐标
        /// </summary>
        public Vector3 GlobalPosition;
        /// <summary>
        /// 全局旋转
        /// </summary>
        public Quaternion GlobalRotation;
        /// <summary>
        /// 按压时的初始位置
        /// </summary>
        public Vector2 RawPosition { get; protected set; }
        Vector2 LastPosition;
        /// <summary>
        /// 鼠标悬停时间,单位毫秒
        /// </summary>
        public int HoverTime { get; protected set; }
        /// <summary>
        /// 按压时间
        /// </summary>
        public long PressTime { get; internal set; }
        /// <summary>
        /// 初次进入的时间
        /// </summary>
        public long EntryTime { get; protected set; }
        /// <summary>
        /// 静止不动时间
        /// </summary>
        public long StayTime { get; protected set; }
        /// <summary>
        /// 按压状态
        /// </summary>
        public bool Pressed { get; internal set; }
        /// <summary>
        /// 焦点状态
        /// </summary>
        public bool Focus { get; internal set; }
        /// <summary>
        /// 用户拖拽造成的速率X
        /// </summary>
        public float VelocityX { get { return mVelocity.x; } set { mVelocity.x = value; } }
        /// <summary>
        /// 用户拖拽造成的速率Y
        /// </summary>
        public float VelocityY { get { return mVelocity.y; } set { mVelocity.y = value; } }
        /// <summary>
        /// 禁止此用户事件
        /// </summary>
        public bool forbid;
        /// <summary>
        /// 开启此项,范围外不会把事件传给子组件
        /// </summary>
        public bool CutRect = false;
        /// <summary>
        /// 强制事件不被子组件拦截
        /// </summary>
        public bool ForceEvent = false;
        /// <summary>
        /// 允许事件穿透
        /// </summary>
        public bool Penetrate = false;
        /// <summary>
        /// 鼠标或触摸是否在UI里面
        /// </summary>
        public bool entry { get; protected set; }
        private int index;
        /// <summary>
        /// 自动调色
        /// </summary>
        public bool AutoColor = true;
        internal Color g_color = Color.white;
        /// <summary>
        /// 联系上下文
        /// </summary>
        public object DataContext;
        /// <summary>
        /// 光标按下
        /// </summary>
        public Action<UserEvent, UserAction> PointerDown;
        /// <summary>
        /// 光标弹起
        /// </summary>
        public Action<UserEvent, UserAction> PointerUp;
        /// <summary>
        /// 光标单击
        /// </summary>
        public Action<UserEvent, UserAction> Click;
        /// <summary>
        /// 光标进入ui区域
        /// </summary>
        public Action<UserEvent, UserAction> PointerEntry;
        /// <summary>
        /// 光标移动
        /// </summary>
        public Action<UserEvent, UserAction> PointerMove;
        /// <summary>
        /// 光标离开此UI区域
        /// </summary>
        public Action<UserEvent, UserAction> PointerLeave;
        /// <summary>
        /// 光标悬停
        /// </summary>
        public Action<UserEvent, UserAction> PointerHover;
        /// <summary>
        /// 长按事件
        /// </summary>
        public Action<UserEvent, UserAction> HoldPress;
        /// <summary>
        /// 滚轮滚动
        /// </summary>
        public Action<UserEvent, UserAction> MouseWheel;
        /// <summary>
        /// 光标拖拽
        /// </summary>
        public Action<UserEvent, UserAction, Vector2> Drag;
        /// <summary>
        /// 光标拖拽完毕
        /// </summary>
        public Action<UserEvent, UserAction, Vector2> DragEnd;
        /// <summary>
        /// 失去焦点
        /// </summary>
        public Action<UserEvent, UserAction> LostFocus;

        UserAction FocusAction;
        /// <summary>
        /// 绑定的UI元素
        /// </summary>
        public UIElement Context;
        /// <summary>
        /// 碰撞器
        /// </summary>
        public EventCollider collider;
        public UserEvent()
        {
            //Rectangular = new Vector3[4];
        }
        /// <summary>
        /// 屏幕坐标转换到局部坐标
        /// </summary>
        /// <param name="v">屏幕坐标</param>
        /// <returns></returns>
        public Vector3 ScreenToLocal(Vector3 v)
        {
            v -= GlobalPosition;
            if (GlobalScale.x != 0)
                v.x /= GlobalScale.x;
            else v.x = 0;
            if (GlobalScale.y != 0)
                v.y /= GlobalScale.y;
            else v.y = 0;
            if (GlobalScale.z != 0)
                v.z /= GlobalScale.z;
            else v.z = 0;
            var q = Quaternion.Inverse(GlobalRotation);
            v = q * v;
            return v;
        }
        public virtual void OnMouseDown(UserAction action)
        {
            if (!action.MultiFocus.Contains(this))
                action.MultiFocus.Add(this);
            Focus = true;
            Pressed = true;
            PressTime = action.EventTicks;
            HoverTime = 0;
            RawPosition = action.CanPosition;
            if (AutoColor)
            {
                Color a;
                a.r = g_color.r * 0.8f;
                a.g = g_color.g * 0.8f;
                a.b = g_color.b * 0.8f;
                a.a = g_color.a;
                Context.MainColor = a;
            }
            if (PointerDown != null)
                PointerDown(this, action);
            entry = true;
            FocusAction = action;
            mVelocity = Vector2.zero;
            UIAudioManager.PointerDown();
        }
        protected virtual void OnMouseUp(UserAction action)
        {
            entry = false;
            if (AutoColor)
            {
                if (!forbid)
                    Context.MainColor = g_color;
            }
            bool press = Pressed;
            Pressed = false;
            if (PointerUp != null)
                PointerUp(this, action);
            UIAudioManager.PointerUp();
            if (press)
            {
                long r = DateTime.Now.Ticks - PressTime;
                if (r <= ClickTime)
                {
                    float x = RawPosition.x - action.CanPosition.x;
                    float y = RawPosition.y - action.CanPosition.y;
                    x *= x;
                    y *= y;
                    x += y;
                    if (x < ClickArea)
                        OnClick(action);
                    UIAudioManager.OnClick();
                }
            }
        }
        protected virtual void OnMouseMove(UserAction action)
        {
            if (!entry)
            {
                entry = true;
                EntryTime = UserAction.Ticks;
                if (PointerEntry != null)
                    PointerEntry(this, action);
                LastPosition = action.CanPosition;
                UIAudioManager.PointerEntry();
            }
            else
            {
                StayTime = action.EventTicks - EntryTime;
                if (action.CanPosition == LastPosition)
                {
                    if(HoverTime < ClickTime)
                    {
                        HoverTime += UserAction.TimeSlice * 10000;
                        if (HoverTime >= ClickTime)
                        {
                            if (Pressed)
                            {
                                if (HoldPress != null)
                                    HoldPress(this, action);
                            }
                            else
                            {
                                if (PointerHover != null)
                                    PointerHover(this, action);
                            }
                        }
                    }
                    else
                    {
                        HoverTime += UserAction.TimeSlice * 10000;
                    }
                }
                else
                {
                    HoverTime = 0;
                    LastPosition = action.CanPosition;
                    if (PointerMove != null)
                        PointerMove(this, action);
                }
            }
        }
        internal virtual void OnMouseLeave(UserAction action)
        {
            entry = false;
            if (AutoColor)
            {
                if (!forbid)
                    Context.MainColor = g_color;
            }
            if (PointerLeave != null)
                PointerLeave(this, action);
            UIAudioManager.PointerLeave();
        }
        internal virtual void OnFocusMove(UserAction action)
        {
            if (Pressed)
                OnDrag(action);
        }
        protected virtual void OnDrag(UserAction action)
        {
            if (action.CanPosition != action.LastPosition)
                if (Drag != null)
                {
                    var v = action.Motion;
                    v.x /= pgs.x;
                    v.x /= action.PhysicalScale;
                    v.y /= pgs.y;
                    v.y /= action.PhysicalScale;
                    Drag(this, action, v);
                }
        }
        internal virtual void OnDragEnd(UserAction action)
        {
            var v = action.Velocities;
            v.x /= GlobalScale.x;
            v.x /= action.PhysicalScale;
            v.y /= GlobalScale.y;
            v.y /= action.PhysicalScale;
            mVelocity = v;
            if (DragEnd != null)
            {
                v = action.Motion;
                v.x /= pgs.x;
                v.x /= action.PhysicalScale;
                v.y /= pgs.y;
                v.y /= action.PhysicalScale;
                DragEnd(this, action, v);
            }
        }
        internal virtual void OnLostFocus(UserAction action)
        {
            Focus = false;
            FocusAction = null;
            if (LostFocus != null)
                LostFocus(this, action);
        }
        internal virtual void OnMouseWheel(UserAction action)
        {
            if (MouseWheel != null)
                MouseWheel(this, action);
        }
        internal virtual void OnClick(UserAction action)
        {
            if (Click != null)
                Click(this,action);
        }
        internal virtual void Initial(FakeStruct mod)
        {
            if (mod == null)
            {
                collider = new UIBoxCollider ();
                return; 
            }
            FakeStruct fs = null;
            unsafe
            {
                fs = UIElementLoader.GetEventData(mod);
             }
            if(fs==null)
            {
                collider = new UIBoxCollider ();
            }
            else
            {
                switch((EventColliderType)fs[0])
                {
                    case EventColliderType.Circle:
                        collider = new UICircleCollider();
                        collider.Initial(fs);
                        break;
                    case EventColliderType.Polygon:
                        collider = new UIPolygonCollider();
                        collider.Initial(fs);
                        break;
                    default:
                        collider = new UIBoxCollider ();
                        break;
                }
            }
        }
        public void RemoveFocus()
        {
            Focus = false;
            Pressed = false;
            if (FocusAction != null)
            {
                FocusAction.RemoveFocus(this);
                FocusAction = null;
            }
        }
        public void Dispose()
        {
            RemoveFocus();
        }
        internal virtual void Update()
        {
            //if (!forbid)
            //    if (!Pressed)
            //        DuringSlide(this);
        }
        /// <summary>
        /// 获取与UI坐标表的相对位置
        /// </summary>
        /// <returns></returns>
        public Vector3 GetOffset()
        {
            Vector3 os = Vector3.zero;
            var size =Context.m_sizeDelta;
            var pivot = Context.Pivot;
            os.x = (0.5f - pivot.x) * size.x;
            os.y = (0.5f - pivot.y) * size.y;
            os.x *= GlobalScale.x;
            os.y *= GlobalScale.y;
            os = GlobalRotation * os;
            return os;
        }
    }
}
