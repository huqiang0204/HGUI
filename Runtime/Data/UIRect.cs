using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Data
{
    public struct UIRect
    {
        float _left, _right, _top, _down;
        Vector2 _pos, _size, _pivot;
        public Vector2 Position { get => _pos;set { _pos = value;Calcul(); } }
        public Vector2 Size { get => _size; set { _size = value; Calcul(); } }
        public Vector2 Pivot { get => _pivot;set { _pivot = value;Calcul(); } }
        public UIRect(Vector2 pos, Vector2 size, Vector2 pivot)
        {
            _pos = pos;
            _size = size;
            _pivot = pivot;
            _left = 0;
            _right = 0;
            _top = 0;
            _down = 0;
            Calcul();
        }
        void Calcul()
        {
            _left = _pos.x + _size.x * -_pivot.x;
            _right = _left + _size.x;
            _top = _pos.y + _size.y * (1 - _pivot.y);
            _down = _top - _size.y;
        }
        public float Left { get => _left; }
        public float Right { get => _right; }
        public float Top { get => _top; }
        public float Down { get => _down; }
    }
}
