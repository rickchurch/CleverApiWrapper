using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleverApiWrapper
{
    /// <summary>
    /// This class respresents the Clever.com object named credentials which is used as a subclass by Clever.com objects Teacher and Student.
    /// </summary>
    public class Credentials
    {
        public string district_username { get; set; }
        public string district_password { get; set; }
    }
}
