using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleverApiWrapper
{
    public class Teacher
    {
        //  NEED TO CONFIRM THESE PROPERTIES - CONFIRMED

        public string district { get; set; }
        public string email { get; set; }
        public string school { get; set; }
        public string sis_id { get; set; }
        public string teacher_number { get; set; }
        public string title { get; set; }
        public Credentials credentials { get; set; }
        public PersonName name { get; set; }
        public string last_modified { get; set; }
        public string created { get; set; }
        public Google_apps google_apps { get; set; }
        // id is the Clever ID that is passed in as part of the url when you req data on this teacher
        public string id { get; set; }
    }
}
