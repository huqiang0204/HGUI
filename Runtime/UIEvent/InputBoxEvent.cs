using huqiang.Core.HGUI;
using huqiang.Core.Line;
using huqiang.Data;
using huqiang.UIComposite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.UIEvent
{
    /// <summary>
    /// 输入框事件
    /// </summary>
    public class InputBoxEvent : UserEvent
    {
        /// <summary>
        /// 输入框复合组件实例
        /// </summary>
        public InputBox input;
        TextBox text;
        protected float overDistance = 500;
        protected float overTime = 0;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mod">数据缓存</param>
        internal override void Initial(FakeStruct mod)
        {
            AutoColor = false;
            text = Context as TextBox;
        }
        public override void OnMouseDown(UserAction action)
        {
            base.OnMouseDown(action);
            input.OnMouseDown(action);
        }
        internal override void OnClick(UserAction action)
        {
            base.OnClick(action);
            input.OnClick(action);
        }
        internal override void OnLostFocus(UserAction action)
        {
            base.OnLostFocus(action);
            input.OnLostFocus(action);
        }
        protected override void OnDrag(UserAction action)
        {
            if (Pressed)
            {
                if (action.CanPosition != action.LastPosition)
                {
                    if (!entry)
                    {
                        float oy = action.CanPosition.y - GlobalPosition.y;
                        float py = GlobalScale.y * text.SizeDelta.y * 0.5f;
                        if (oy > 0)
                            oy -= py;
                        else oy += py;
                        if (oy > overDistance)
                            oy = overDistance;
                        float per = 5000 / oy;
                        if (per < 0)
                            per = -per;
                        overTime += UserAction.TimeSlice;
                        if (overTime >= per)
                        {
                            overTime -= per;
                            if (oy > 0)
                            {
                                text.StartLine--;
                            }
                            else
                            {
                                text.StartLine++;

                            }
                        }
                    }
                    input.OnDrag(action);
                }
            }
            base.OnDrag(action);
        }
        internal override void OnMouseWheel(UserAction action)
        {
            float oy = action.MouseWheelDelta;
            if (oy > 0)
            {
                text.StartLine++;
            }
            else
            {
                text.StartLine--;
            }
            base.OnMouseWheel(action);
        }
    }
}
