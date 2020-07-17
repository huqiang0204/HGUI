using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huqiang.Data
{
    [Serializable]
    public class Msg
    {
        public Int32 Type;
        public Int32 Cmd;
        public Int32 Error;
        public string Args;
    }
}
