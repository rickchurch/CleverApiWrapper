using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleverApiWrapper
{
    /// <summary>
    /// This class respresents the Clever.com object named Student.
    /// </summary>
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
        public PersonName name { get; set; }
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
