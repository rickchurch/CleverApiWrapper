using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleverApiWrapper
{
    /// <summary>
    /// This class respresents the Clever.com object named Contact - specifically student contact.
    /// </summary>
    public class Contact
    {
        public string email { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string student { get; set; } // id of student
        public string type { get; set; } // type of contact (relationship to student)
        public string id { get; set; } // contact id
    }
}
