using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleverApiWrapper
{
    /// <summary>
    /// This is return object for this library's DataRequest method. WrappedData contains lists of Clever.com objects - Districts, Schools, 
    /// Teachers, Sections, Students, & Events.  If the user passed in arguments do not indicate a request for any of the Clever.com objects,
    ///  WrappedData will simply contain an empty list of that object.
    /// </summary>
    public class WrappedData
    {
        public List<Event> Events { get; set; }
        public List<Section> Sections { get; set; }
        public List<Teacher> Teachers { get; set; }
        public List<School> Schools { get; set; }
        public List<District> Districts { get; set; }
        public List<Student> Students { get; set; }
        public List<Contact> Contacts { get; set; }
        public List<Admin> Admins { get; set; }
        public string Grade_Levels { get; set; }

        public WrappedData()
        {
            Events = new List<Event>();
            Sections = new List<Section>();
            Teachers = new List<Teacher>();
            Schools = new List<School>();
            Districts = new List<District>();
            Students = new List<Student>();
            Contacts = new List<Contact>();
            Admins = new List<Admin>();
            Grade_Levels = string.Empty;
        }
    }
}
