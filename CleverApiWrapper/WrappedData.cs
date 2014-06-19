using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleverApiWrapper
{
    public class WrappedData
    {
        public List<Event> Events { get; set; }
        public List<Section> Sections { get; set; }
        public List<Teacher> Teachers { get; set; }
        public List<School> Schools { get; set; }
        public List<District> Districts { get; set; }
        public List<Student> Students { get; set; }

        public WrappedData()
        {
            Events = new List<Event>();
            Sections = new List<Section>();
            Teachers = new List<Teacher>();
            Schools = new List<School>();
            Districts = new List<District>();
            Students = new List<Student>();
        }
    }
}
