using huqiang.Core.HGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.UIEvent
{
    public class UserAction
    {
        /// <summary>
        /// 默认的加速时间,单位毫秒
        /// </summary>
        public static float Accelerationtime = 60;
        /// <summary>
        /// 时间片,单位毫秒
        /// </summary>
        public static int TimeSlice;
        /// <summary>
        /// 上一帧时间,单位毫秒
        /// </summary>
        public static int LastTime;
        /// <summary>
        /// 当前时钟周期
        /// </summary>
        public static long Ticks;
        /// <summary>
        /// 状态更新
        /// </summary>
        public static void Update()
        {
            DateTime now = DateTime.Now;
            Ticks = now.Ticks;
            int s = now.Millisecond;
            int t = s - LastTime;
            if (t < 0)
                t += 1000;
            TimeSlice = t;
            LastTime = s;
        }
        /// <summary>
        /// 事件id
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// 上一帧屏幕坐标
        /// </summary>
        public Vector2 LastPosition;
        /// <summary>
        /// 当前帧画布坐标
        /// </summary>
        public Vector2 CanPosition;
        /// <summary>
        /// 当前帧画布坐标
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// 当前帧与上一帧的运动距离
        /// </summary>
        public Vector2 Motion;
        Vector2 m_Velocities;
        /// <summary>
        /// 10内移动平均速率
        /// </summary>
        public Vector2 Velocities { get { return m_Velocities; } }
        /// <summary>
        /// 是否移动了
        /// </summary>
        public bool IsMoved { get; set; }
        /// <summary>
        /// 手指在操作
        /// </summary>
        public bool FingerStationary { get; set; }
        /// <summary>
        /// 鼠标左键按下状态
        /// </summary>
        public bool IsLeftButtonDown { get; set; }
        /// <summary>
        /// 鼠标左键按下
        /// </summary>
        public bool isPressed { get; set; }
        /// <summary>
        /// 鼠标左键弹起
        /// </summary>
        public bool IsLeftButtonUp { get; set; }
        /// <summary>
        /// 按下时的最初位置
        /// </summary>
        public Vector2 rawPosition { get; set; }
        /// <summary>
        /// 触控的tapCount
        /// </summary>
        public int tapCount { get; set; }
        public float altitudeAngle { get; set; }
        public float azimuthAngle { get; set; }
        public float radius { get; set; }
        public float radiusVariance { get; set; }
        /// <summary>
        /// 鼠标按压世间
        /// </summary>
        public float PressTime { get; set; }
        /// <summary>
        /// 当前事件时钟周期
        /// </summary>
        public long EventTicks { get; set; }
        Vector3[] FramePos = new Vector3[16];
        int Frame;
        List<UserEvent> LastEntry;
        /// <summary>
        /// 当前包含鼠标的所有UI
        /// </summary>
        public List<UserEvent> CurrentEntry;
        List<UserEvent> LastFocus;
        /// <summary>
        /// 当前拥有焦点的所有UI
        /// </summary>
        public List<UserEvent> MultiFocus;
        public UserAction(int id)
        {
            Id = id;
            LastEntry = new List<UserEvent>();
            CurrentEntry = new List<UserEvent>();
            LastFocus = new List<UserEvent>();
            MultiFocus = new List<UserEvent>();
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Clear()
        {
            LastEntry.Clear();
            CurrentEntry.Clear();
            LastFocus.Clear();
            MultiFocus.Clear();
            IsLeftButtonDown = false;
            IsRightButtonDown = false;
            isPressed = false;
            IsLeftButtonUp = false;
            IsRightButtonUp = false;
            IsRightButtonUp = false;
            IsMoved = false;
            FingerStationary = false;
        }
        /// <summary>
        /// 移除焦点所有UI
        /// </summary>
        public void ReleaseFocus()
        {
            LastEntry.Clear();
            CurrentEntry.Clear();
            LastFocus.Clear();
            MultiFocus.Clear();
        }
        public float MouseWheelDelta { get; set; }
        public bool IsMouseWheel { get; set; }
        public bool IsMiddleButtonDown { get; set; }
        public bool IsRightButtonDown { get; set; }
        public bool IsMiddleButtonUp { get; set; }
        public bool IsRightButtonUp { get; set; }
        public bool IsRightPressed { get; set; }
        public bool IsMiddlePressed { get; set; }
        public bool IsActive { get; set; }
        void CalculVelocities()
        {
            if (PressTime > Accelerationtime)
            {
                int s = Frame;
                float time = 0;
                for (int i = 0; i < 6; i++)
                {
                    time += FramePos[s].z;
                    if (time >= Accelerationtime)
                        break;
                    s--;
                    if (s < 0)
                        s = 15;
                }
                float x = FramePos[Frame].x - FramePos[s].x;
                float y = FramePos[Frame].y - FramePos[s].y;
                m_Velocities.x = x / time;
                m_Velocities.y = y / time;
            }
            else m_Velocities = Vector2.zero;
        }
        /// <summary>
        /// 载入触摸信息
        /// </summary>
        /// <param name="touch"></param>
        public void LoadFinger(ref Touch touch)
        {
            LastPosition = CanPosition;
            Id = touch.fingerId;
            switch (touch.phase)
            {
                case TouchPhase.Began://pointer down
                    IsLeftButtonDown = true;
                    FingerStationary = false;
                    isPressed = true;
                    IsLeftButtonUp = false;
                    IsMoved = false;
                    break;
                case TouchPhase.Moved:
                    IsLeftButtonDown = false;
                    isPressed = true;
                    IsLeftButtonUp = false;
                    IsMoved = true;
                    FingerStationary = false;
                    break;
                case TouchPhase.Ended://pointer up
                    IsLeftButtonDown = false;
                    isPressed = false;
                    IsLeftButtonUp = true;
                    IsMoved = false;
                    FingerStationary = false;
                    break;
                case TouchPhase.Stationary://悬停
                    IsLeftButtonDown = false;
                    isPressed = true;
                    IsLeftButtonUp = false;
                    IsMoved = false;
                    FingerStationary = true;
                    break;
                default:
                    IsLeftButtonDown = false;
                    //isPressed = false;
                    //IsLeftButtonUp = false;
                    IsMoved = false;
                    FingerStationary = false;
                    break;
            }
            if (IsLeftButtonDown)
            {
                EventTicks = Ticks;
                PressTime = 0;
            }
            else PressTime += TimeSlice;
            Motion = touch.deltaPosition;
            tapCount = touch.tapCount;
            rawPosition = touch.rawPosition;
            Position = touch.position;
            float x = Screen.width;
            x *= 0.5f;
            float y = Screen.height;
            y *= 0.5f;
            CanPosition.x = Position.x - x;
            CanPosition.y = Position.y - y;
            float ps = HCanvas.MainCanvas.PhysicalScale;
            CanPosition.x /= ps;
            CanPosition.y /= ps;

            FramePos[Frame].x = Position.x;
            FramePos[Frame].y = Position.y;
            FramePos[Frame].z = TimeSlice;
            CalculVelocities();
            Frame++;
            if (Frame >= 16)
                Frame = 0;
            altitudeAngle = touch.altitudeAngle;
            azimuthAngle = touch.azimuthAngle;
            radius = touch.radius;
            radiusVariance = touch.radiusVariance;
        }
        /// <summary>
        /// 载入鼠标信息
        /// </summary>
        public void LoadMouse()
        {
            LastPosition = CanPosition;
            IsActive = true;
            MouseWheelDelta = Input.mouseScrollDelta.y;
            if (MouseWheelDelta != 0)
                IsMouseWheel = true;
            else IsMouseWheel = false;
            IsLeftButtonDown = Input.GetMouseButtonDown(0);
            IsRightButtonDown = Input.GetMouseButtonDown(1);
            IsMiddleButtonDown = Input.GetMouseButtonDown(2);
            IsLeftButtonUp = Input.GetMouseButtonUp(0);
            IsRightButtonUp = Input.GetMouseButtonUp(1);
            IsMiddleButtonUp = Input.GetMouseButtonUp(2);
            isPressed = Input.GetMouseButton(0);
            IsRightPressed = Input.GetMouseButton(1);
            IsMiddlePressed = Input.GetMouseButton(2);
            if (IsLeftButtonDown | IsRightButtonDown | IsMiddleButtonDown)
            {
                EventTicks = Ticks;
                PressTime = 0;
                rawPosition = Input.mousePosition;
            }
            else { PressTime += TimeSlice; }
            IsMoved = false;
            float x = Input.mousePosition.x;
            Motion.x = x - Position.x;
            if (Motion.x != 0)
            {
                IsMoved = true;
            }
            Position.x = x;
            float y = Input.mousePosition.y;
            Motion.y = y - Position.y;
            if (Motion.y != 0)
            {
                IsMoved = true;
            }
            Position.y = y;
            //x = Screen.width;
            //x *= 0.5f;
            //y = Screen.height;
            //y *= 0.5f;
            //CanPosition.x = Position.x - x;
            //CanPosition.y = Position.y - y;
            //float ps = HCanvas.MainCanvas.PhysicalScale;
            //CanPosition.x /= ps;
            //CanPosition.y /= ps;
            CanPosition = HCanvas.MainCanvas.ScreenToCanvasPos(Position);
            FramePos[Frame].x = Position.x;
            FramePos[Frame].y = Position.y;
            FramePos[Frame].z = TimeSlice;
            CalculVelocities();
            Frame++;
            if (Frame >= 16)
                Frame = 0;
        }
        /// <summary>
        /// 复制事件
        /// </summary>
        /// <param name="action"></param>
        /// <param name="FormSize"></param>
        public void CopyAction(UserAction action, Vector2 FormSize)
        {
            LastPosition = CanPosition;
            IsActive = true;
            MouseWheelDelta = action.MouseWheelDelta;
            if (MouseWheelDelta != 0)
                IsMouseWheel = true;
            else IsMouseWheel = false;
            IsLeftButtonDown = action.IsLeftButtonDown;
            IsRightButtonDown = action.IsRightButtonDown;
            IsMiddleButtonDown = action.IsMiddleButtonDown;
            IsLeftButtonUp = action.IsLeftButtonUp;
            IsRightButtonUp = action.IsRightButtonUp;
            IsMiddleButtonUp = action.IsMiddleButtonUp;
            isPressed = action.isPressed;
            IsRightPressed = action.IsRightPressed;
            IsMiddlePressed = action.IsMiddlePressed;
            if (IsLeftButtonDown | IsRightButtonDown | IsMiddleButtonDown)
            {
                EventTicks = Ticks;
                PressTime = 0;
                rawPosition = action.rawPosition;
            }
            else { PressTime += TimeSlice; }
            IsMoved = false;
            float x = action.Position.x;
            Motion.x = x - Position.x;
            if (Motion.x != 0)
            {
                IsMoved = true;
            }
            Position.x = x;
            float y = action.Position.y;
            Motion.y = y - Position.y;
            if (Motion.y != 0)
            {
                IsMoved = true;
            }
            Position.y = y;
            x = FormSize.x;
            x *= 0.5f;
            y = FormSize.y;
            y *= 0.5f;
            CanPosition.x = Position.x - x;
            CanPosition.y = Position.y - y;
            FramePos[Frame].x = Position.x;
            FramePos[Frame].y = Position.y;
            FramePos[Frame].z = TimeSlice;
            CalculVelocities();
            Frame++;
            if (Frame >= 16)
                Frame = 0;
        }
        /// <summary>
        /// 派发事件
        /// </summary>
        /// <param name="pipeLine">ui流水线</param>
        public void Dispatch(HGUIElement[] pipeLine)
        {
            if (IsLeftButtonDown | IsRightButtonDown | IsMiddleButtonDown)
            {
                List<UserEvent> tmp = MultiFocus;
                LastFocus.Clear();
                MultiFocus = LastFocus;
                LastFocus = tmp;
            }
            UserEvent.DispatchEvent(this, pipeLine);
            if (IsLeftButtonDown | IsRightButtonDown | IsMiddleButtonDown)
            {
                for (int i = 0; i < LastFocus.Count; i++)
                {
                    var f = LastFocus[i];
                    for (int j = 0; j < MultiFocus.Count; j++)
                    {
                        if (f == MultiFocus[j])
                            goto label2;
                    }
                    if (!f.Pressed)
                        f.OnLostFocus(this);
                    label2:;
                }
            }
            else if (IsLeftButtonUp | IsRightButtonUp | IsMiddleButtonUp)
            {
                for (int i = 0; i < MultiFocus.Count; i++)
                {
                    var f = MultiFocus[i];
                    f.Pressed = false;
                    f.OnDragEnd(this);
                }
            }
            else
            {
                for (int i = 0; i < MultiFocus.Count; i++)
                    MultiFocus[i].OnFocusMove(this);
            }
            if (IsMouseWheel)
            {
                for (int i = 0; i < MultiFocus.Count; i++)
                {
                    var f = MultiFocus[i];
                    f.OnMouseWheel(this);
                }
            }
            CheckMouseLeave();
        }
        void CheckMouseLeave()
        {
            for (int i = 0; i < LastEntry.Count; i++)
            {
                var eve = LastEntry[i];
                if (eve != null)
                {
                    for (int j = 0; j < CurrentEntry.Count; j++)
                    {
                        if (CurrentEntry[j] == eve)
                            goto label;
                    }
                    eve.OnMouseLeave(this);
                label:;
                }
            }
            List<UserEvent> tmp = LastEntry;
            tmp.Clear();
            LastEntry = CurrentEntry;
            CurrentEntry = tmp;
        }
        /// <summary>
        /// 该用户事件是否存在焦点
        /// </summary>
        /// <param name="eve">用户事件实例</param>
        /// <returns></returns>
        public bool ExistFocus(UserEvent eve)
        {
            return MultiFocus.Contains(eve);
        }
        /// <summary>
        /// 添加焦点
        /// </summary>
        /// <param name="eve">用户事件实例</param>
        public void AddFocus(UserEvent eve)
        {
            if (MultiFocus.Contains(eve))
                return;
            MultiFocus.Add(eve);
            eve.Pressed = isPressed;
            if (eve.Pressed)
                eve.PressTime = EventTicks;
        }
        /// <summary>
        /// 移除焦点
        /// </summary>
        /// <param name="eve">用户事件实例</param>
        public void RemoveFocus(UserEvent eve)
        {
            LastEntry.Remove(eve);
            CurrentEntry.Remove(eve);
            LastFocus.Remove(eve);
            MultiFocus.Remove(eve);
            eve.Pressed = false;
        }
    }
}
