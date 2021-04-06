using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetScan.TetScan.Utils
{
    class ActionRequest
    {
        public string action;
        public object payload;
        public Dictionary<string, string> options;
    }
}
