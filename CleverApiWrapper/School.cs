using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleverApiWrapper
{
    public class School
    {
        public string district { get; set; }
        public string high_grade { get; set; }
        public string low_grade { get; set; }
        public string name { get; set; }
        public string nces_id { get; set; }
        public string phone { get; set; }
        public string school_number { get; set; }
        public string sis_id { get; set; }
        public string state_id { get; set; }
        public SchoolLocation location { get; set; }
        public Principal principal { get; set; }
        public string last_modified { get; set; }
        public string created { get; set; }
        public string id { get; set; }
    }

    public class SchoolLocation
    {
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
    }

    public class Principal
    {
        public string name { get; set; }
        public string email { get; set; }
    }
}
