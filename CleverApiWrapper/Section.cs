using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleverApiWrapper
{
    public class Section
    {
        //  NEED TO CONFIRM THESE PROPERTIES  !!!!!!!!!!!!

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
        public SectionTerm term { get; set; }
        //public string start_date { get; set; }
        //public string end_date { get; set; }
        public List<string> students { get; set; }
        public string last_modified { get; set; }
        public string created { get; set; }
        public string id { get; set; }
    }

    public class SectionTerm
    {
        public string name { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
    }
    /*
    {"course_name":"Group Guidance",
"course_number":"101",
"district":"4fd43cc56d11340000000005",
"grade":"10",
"name":"Group Guidance - 101 - B. Greene (Section)",
"period":"0",
"school":"530e595026403103360ff9fd",
"sis_id":"581",
"subject":"homeroom/advisory",
"teacher":"530e5955d50c310f36112c11",
"term":{
	"name":"Y1",
	"start_date":"2012-08-01T00:00:00.000Z",
	"end_date":"2013-06-01T00:00:00.000Z"},
"students":["530e5961049e75a9262cffd9","530e5961049e75a9262d0010","530e5961049e75a9262d004e","530e5961049e75a9262d0080","530e5962049e75a9262d0156","530e5963049e75a9262d01b7","530e5963049e75a9262d0253","530e5966049e75a9262d0418","530e5966049e75a9262d0475","530e5966049e75a9262d04b4","530e5966049e75a9262d04ba","530e5967049e75a9262d0503","530e5968049e75a9262d05e9","530e5968049e75a9262d061f","530e5968049e75a9262d0632","530e5968049e75a9262d0647"],

"last_modified":"2014-02-26T21:15:37.930Z",
"created":"2014-02-6T21:15:37.927Z",
"id":"530e5979049e75a9262d0af2"},
     * */
}

