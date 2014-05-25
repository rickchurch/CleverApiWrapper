using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleverApiWrapper
{
    public class Student
    {
        public string district { get; set; }
        public string dob { get; set; }
        public string ell_status { get; set; }
        public string email { get; set; }
        public string frl_status { get; set; }
        public string gender { get; set; }
        public string grade { get; set; }
        public string hispanic_ethnicity { get; set; }
        public string iep_status { get; set; }
        public string race { get; set; }
        public string school { get; set; }
        public string sis_id { get; set; }
        public string state_id { get; set; }
        public string student_number { get; set; }
        public Credentials credentials { get; set; }
        public StudentLocation location { get; set; }
        public Name name { get; set; }
        public string last_modified { get; set; }
        public string created { get; set; }
        public Google_apps google_apps { get; set; }
        public string id { get; set; }
    }

    public class StudentLocation
    {
        public string zip { get; set; }
    }

}

/*
"district":"4fd43cc56d11340000000005",
"dob":"2/11/2007",
"ell_status":"N",
"email":"z.steve@example.net",
"frl_status":"Free",
"gender":"M",
"grade":"Kindergarten",
"hispanic_ethnicity":"Y",
"iep_status":"N",
"race":"Black or African American",
"school":"530e595026403103360ff9fe",
"sis_id":"100095233",
"state_id":"231786324",
"student_number":"100095233",
"credentials":{
	"district_username":"stevez33",
	"district_password":"auyik3tiTieL"},
"location":{
	"zip":"10459"},
"name":{"
	first":"Steve",
	"middle":"G",
	"last":"Ziemann"},
"last_modified":"2014-02-26T21:15:12.350Z",
"created":"2014-02-26T21:15:12.346Z",
"google_apps":{},
"id":"530e5960049e75a9262cff1d"},
*/