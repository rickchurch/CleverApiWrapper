using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleverApiWrapper
{
    public class Section
    {
        public string course_name { get; set; }
        public string course_number { get; set; }
        public string district { get; set; }
        public string grade { get; set; }
        public string name { get; set; }
        public string period { get; set; }
        public string school { get; set; }
        public string sis_id { get; set; }
        public string subject { get; set; }
        public string teacher { get; set; }
        public string term { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public List<string> students { get; set; }
        public string last_modified { get; set; }
        public string created { get; set; }
        public string id { get; set; }
    }
}

