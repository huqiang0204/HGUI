using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huqiang.Unit
{
    public class UCollider2D
    {
        public Unit Target;
        public virtual void Update()
        {
        }
        public virtual bool Check(UCollider2D other)
        {
            return false;
        }
    }
}
