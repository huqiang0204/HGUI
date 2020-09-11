using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace huqiang.UIComposite
{
    public class CheckBox
    {
        List<UserEvent> events = new List<UserEvent>();
        public UserEvent Checked { get; private set; }
        public Action<CheckBox> SelectChanged;
        public void AddEvent(UserEvent user)
        {
            if (events.Contains(user))
                return;
            user.Click = Click;
            var son = user.Context.transform.GetChild(0);
            if (son != null)
                son.gameObject.SetActive(false);
            events.Add(user);
            if(events.Count==1)
            {
                Checked = user;
                var s = user.Context.transform.GetChild(0);
                if (s != null)
                    s.gameObject.SetActive(true);
            }
        }
        public void RemoveEvent(UserEvent user)
        {
            events.Remove(user);
            if (Checked == user)
            {
                if (events.Count > 0)
                {
                    Checked = events[0];
                    var son = Checked.Context.transform.GetChild(0);
                    if (son != null)
                        son.gameObject.SetActive(true);
                    if (SelectChanged != null)
                        SelectChanged(this);
                }
            }
        }
        void Click(UserEvent user,UserAction action)
        {
            if (Checked != null)
            {
                var son = Checked.Context.transform.GetChild(0);
                if (son != null)
                    son.gameObject.SetActive(false);
            }
            Checked = user;
            var s = user.Context.transform.GetChild(0);
            if (s != null)
                s.gameObject.SetActive(true);
            if (SelectChanged != null)
                SelectChanged(this);
        }
        public void SetChecked(UserEvent user)
        {
            if (!events.Contains(user))
                events.Add(user);
            if(Checked!=null)
            {
                var son = Checked.Context.transform.GetChild(0);
                if (son != null)
                    son.gameObject.SetActive(false);
            }
            Checked = user;
            var s = user.Context.transform.GetChild(0);
            if (s != null)
                s.gameObject.SetActive(true);
        }
    }
}
