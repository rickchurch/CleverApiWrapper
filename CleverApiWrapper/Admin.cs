using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleverApiWrapper
{
    /// <summary>
    /// This class respresents the Clever.com object named Admin - specifically District Admin.
    /// </summary>
    public class Admin
    {
        public string email { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string id { get; set; } // contact id
    }
}
