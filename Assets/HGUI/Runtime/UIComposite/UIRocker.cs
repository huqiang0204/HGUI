using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class UIRocker : Composite
    {
        public Transform Nob;
        public UserEvent callBack;
        float _r;
        public float Radius { get { return _r; }set { if (value <= 0) value = 0.01f; _r = value;_s = _r * _r; } }
        public float Slider { get { return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y) / _r; } }
        public enum Direction
        {
            None,Up,RifhtUp,Right,RightDown,Down,LeftDown,Left,LeftUp
        }
        float _angle;
        float _s;
        public float Angle { get { return _angle; } set {
                _angle = value;
                _dir = (Direction)(value / 45);
                vector = MathH.Tan2(_angle);
            } }
        Direction _dir;
        public Direction direction { get { return _dir; }
            set { _dir = value;
                _angle = (((int)_dir) - 1) * 45;
                vector = MathH.Tan2(_angle);
            } }
        public Vector2 vector;
        public Action<UIRocker> Rocking;
        public override void Initial(FakeStruct fake,UIElement script)
        {
            base.Initial(fake,script);
            callBack = script.RegEvent<UserEvent>();
            callBack.Drag = Draging;
            callBack.DragEnd = DragEnd;
            callBack.PointerDown = PointDown;
            callBack.IsCircular = true;
            _r = Enity.SizeDelta.x * 0.5f;
            if (_r <= 0)
                _r = 0.01f;
            _s = _r * _r;
            Nob = Enity.transform.Find("Nob");
        }
        void Draging(UserEvent back, UserAction action, Vector2 v)
        {
           float x = action.CanPosition.x - back.GlobalPosition.x;
           float y = action.CanPosition.y - back.GlobalPosition.y;
            x /= back.GlobalScale.x;
            y /= back.GlobalScale.y;
            float sx = x * x + y * y;
            if (sx > _s)
            {
                float r = Mathf.Sqrt(_s / sx);
                x *= r;
                y *= r;
            }
            float al = MathH.atan(x, y);
            _angle = al;
            al += 22.5f;
            if (al > 360f)
                al -= 360f;
            al /= 45f;
            int index = (Int32)al;
            index++;
            _dir = (Direction)index;
            vector.x = x;
            vector.y = y;
            if (Nob != null)
            {
                Nob.localPosition = new Vector3(x,y,0);
            }
            if (Rocking != null)
                Rocking(this);
        }
        void DragEnd(UserEvent back, UserAction action, Vector2 v)
        {
            if (Nob != null)
            {
                Nob.transform.localPosition = Vector3.zero;
            }
            _angle = 0;
            _dir = Direction.None;
            vector.x = 0;
            vector.y = 0;
            if (Rocking != null)
                Rocking(this);
        }
        void PointDown(UserEvent back, UserAction action)
        {
            float x = action.CanPosition.x - back.GlobalPosition.x;
            float y = action.CanPosition.y - back.GlobalPosition.y;
            x /= back.GlobalScale.x;
            y /= back.GlobalScale.y;
            float al = MathH.atan(x, y);
            _angle = al;
            al += 22.5f;
            if (al > 360f)
                al -= 360f;
            al /= 45f;
            int index = (Int32)al;
            index++;
            _dir = (Direction)index;
            if (Nob != null)
            {
                Nob.localPosition = new Vector3(x,y,0);
            }
            vector.x = x;
            vector.y = y;
            if (Rocking != null)
                Rocking(this);
        }
    }
}
