using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleverApiWrapper
{
    /// <summary>
    /// This class respresents the Clever.com object named name.  PersonName is used as a subclass by Clever.com objects Teacher and Student.
    /// </summary>
    public class PersonName
    {
        public string first { get; set; }
        public string middle { get; set; }
        public string last { get; set; }
    }
}
