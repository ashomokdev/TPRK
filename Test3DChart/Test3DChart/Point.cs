using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test3DChart
{
    public class Point
    {
        public string Action { get; set; }

        public int Consequence { get; set; }

        public Point(string action, int consequence)
        {
            this.Action = action;
            this.Consequence = consequence;
        }

        public Point()
        {            
        }
    }
}
