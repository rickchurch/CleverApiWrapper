using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;  // for JavaScriptSerializer  & had to add ref  system.web.extensions
using System.Collections;
using System.Reflection;

namespace CleverApiWrapper
{
    /// <summary>
    /// This class takes the passed in raw JSON msg (returned from Clever.com) and parses out the various objects (Districts, 
    /// Schools, ect) and puts them into a parent container object WrappedData.
    /// </summary>
    class Parser
    {
        Logger mLogger;
        WrappedData mWrappedData;

        internal Parser(Logger logger)
        {
            mLogger = logger;
            mWrappedData = new WrappedData();
        }

        internal WrappedData ParseJsonMsg(string msg, string knownObject = "")
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            var jss = new JavaScriptSerializer();
            var masterDict = jss.Deserialize<Dictionary<string, dynamic>>(msg);
            foreach (KeyValuePair<string, dynamic> kvp in masterDict)
            {
                if (kvp.Key == "data")
                {
                    bool compoundData = false;
                    if (kvp.Value.GetType() == typeof(ArrayList))
                    {
                        if (string.IsNullOrEmpty(knownObject))
                        {
                            mLogger.Log(methodName, string.Format("Unexpected error!!  I don't know what the Clever object type is."), 1, true);
                            return mWrappedData;
                        }
                        else
                        {
                            ProcessArrayObject(kvp.Value, knownObject);
                        }
                        continue;
                    }
                    else
                    {
                        mLogger.Log(methodName, string.Format("Upper most level key: {0}", kvp.Key), 3);
                        foreach (KeyValuePair<string, dynamic> kvp2 in kvp.Value)
                        {
                            //if (kvp2.Key == "districts" || kvp2.Key == "schools" || kvp2.Key == "teachers" || kvp2.Key == "sections" || kvp2.Key == "students" || kvp2.Key == "events")
                            if (kvp2.Key == "districts" || kvp2.Key == "schools" || kvp2.Key == "teachers" || kvp2.Key == "sections" || kvp2.Key == "students")
                            {
                                compoundData = true;
                                string targetObjectType = kvp2.Key;
                                mLogger.Log(methodName, string.Format("Found object data for: {0}", targetObjectType), 2, true);
                                foreach (KeyValuePair<string, dynamic> kvp3 in kvp2.Value)
                                {
                                    mLogger.Log(methodName, string.Format("Looking for key=data.  This key: {0}", kvp3.Key), 3);
                                    if (kvp3.Key == "data")
                                    {
                                        if (kvp3.Value.GetType() == typeof(ArrayList))
                                        {
                                            ProcessArrayObject(kvp3.Value, targetObjectType);
                                        }
                                        else
                                        {
                                            mLogger.Log(methodName, string.Format("Unexpected error!!  I don't know what the Clever object type is."), 1, true);
                                            return mWrappedData;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if ((kvp.Value.GetType() == typeof(Dictionary<string, dynamic>)) && !compoundData)
                    {
                        mLogger.Log(methodName, string.Format("Putting masterDict into an array object and sending it on to be processed."), 1, true);
                        if (kvp.Key == "data")
                        {
                            ArrayList arrList = new ArrayList();
                            arrList.Add(masterDict);
                            ProcessArrayObject(arrList, knownObject);
                        }
                    }

                }
            }

            mLogger.Log(methodName, "", 2);
            mLogger.Log(methodName, string.Format("Completed processing to parse raw Clever message."), 2);
            mLogger.Log(methodName, string.Format("Found {0} districts.", mWrappedData.Districts.Count), 2);
            mLogger.Log(methodName, string.Format("Found {0} schools.", mWrappedData.Schools.Count), 2);
            mLogger.Log(methodName, string.Format("Found {0} teachers.", mWrappedData.Teachers.Count), 2);
            mLogger.Log(methodName, string.Format("Found {0} sections.", mWrappedData.Sections.Count), 2);
            mLogger.Log(methodName, string.Format("Found {0} students.", mWrappedData.Students.Count), 2);
            mLogger.Log(methodName, string.Format("Found {0} student contacts.", mWrappedData.Contacts.Count), 2);
            mLogger.Log(methodName, string.Format("Found {0} district admins.", mWrappedData.Admins.Count), 2);
            return mWrappedData;
        }

        void ProcessArrayObject(ArrayList targetObjectList, string targetObjectType)
        {
            bool isDict = false;
            foreach (var item in targetObjectList)
            {
                if (item != null)
                {
                    Type valueType = item.GetType();
                    if (valueType.IsGenericType)
                    {
                        Type baseType = valueType.GetGenericTypeDefinition();
                        if (baseType == typeof(Dictionary<,>))
                        {
                            isDict = true;
                        }
                    }
                    if (targetObjectType == "contacts")
                    {
                        bool instantiateSuccess = InstantiateContact(item);
                    }
                    else if (targetObjectType == "admins")
                    {
                        bool instantiateSuccess = InstantiateAdmin(item);
                    }
                    else if (targetObjectType == "grade_levels")
                    {
                        // the sample data (with demo token) seemed to only return a single number under "data" key
                        //    so this may need more work when additional data is available
                        mWrappedData.Grade_Levels = item.ToString();
                    }
                    else if (isDict)
                    {
                        Dictionary<string, dynamic> targetDataDict = (Dictionary<string, dynamic>)item;
                        foreach (KeyValuePair<string, dynamic> targetData in targetDataDict)
                        {
                            if (targetData.Key == "data" || targetObjectType == "contacts")
                            {
                                Tuple<string, string> emptyTuple = new Tuple<string, string>("", "");
                                if (targetObjectType == "districts" || targetObjectType == "status")
                                {
                                    bool instantiateSuccess = InstantiateDistrict(targetData.Value, emptyTuple);
                                }
                                else if (targetObjectType == "schools")
                                {
                                    bool instantiateSuccess = InstantiateSchool(targetData.Value, emptyTuple);
                                }
                                else if (targetObjectType == "teachers")
                                {
                                    bool instantiateSuccess = InstantiateTeacher(targetData.Value, emptyTuple);
                                }
                                else if (targetObjectType == "sections")
                                {
                                    bool instantiateSuccess = InstantiateSection(targetData.Value, emptyTuple);
                                }
                                else if (targetObjectType == "students")
                                {
                                    bool instantiateSuccess = InstantiateStudent(targetData.Value, emptyTuple);
                                }
                                else if (targetObjectType == "events")
                                {
                                    GetEventData(targetData.Value);
                                }
                            }
                        }
                    }
                }
            }
        }

        bool InstantiateDistrict(dynamic dataDict, Tuple<string, string> when_id)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            mLogger.Log(methodName, "", 3);

            bool success = true;

            District district = new District();

            // Get each dp (datapoint) found in dataDict
            foreach (KeyValuePair<string, dynamic> dpParent in dataDict)
            {
                if (dpParent.Value != null)
                {
                    Type valueType = dpParent.Value.GetType();
                    if (valueType.IsGenericType)
                    {
                        mLogger.Log(methodName, string.Format("ALERT !  Condition not expected. valueType of dpParent: {0}", valueType.FullName), 1);
                    }
                    else if (valueType.Name == "String")
                    {
                        mLogger.Log(methodName, string.Format("Key: {0}     Value: {1}", dpParent.Key, dpParent.Value), 3);

                        PropertyInfo pi = district.GetType().GetProperty(dpParent.Key);
                        ValidateClassProperty(pi, dpParent.Key, "District");
                        pi.SetValue(district, Convert.ChangeType(dpParent.Value, pi.PropertyType), null);
                    }
                }
            }
            //  check whether we are instantiating for event data and if so, set event_type and id
            if (when_id.Item1 == "current_attributes")
            {
                district.event_type = when_id.Item1;
            }
            else if (when_id.Item1 == "previous_attributes")
            {
                district.event_type = when_id.Item1;
                district.id = when_id.Item2;
            }
            mWrappedData.Districts.Add(district);

            return success;
        }

        bool InstantiateSchool(dynamic dataDict, Tuple<string, string> when_id)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            mLogger.Log(methodName, "", 3);

            bool success = true;

            School school = new School();
            SchoolLocation loc = new SchoolLocation();
            Principal principal = new Principal();

            // Get each dp (datapoint) found in dataDict
            foreach (KeyValuePair<string, dynamic> dpParent in dataDict)
            {
                if (dpParent.Value != null)
                {
                    Type valueType = dpParent.Value.GetType();
                    if (valueType.IsGenericType)
                    {
                        // This value item is not just another string value but (probably) another dictionary
                        Type baseType = valueType.GetGenericTypeDefinition();
                        if (baseType == typeof(Dictionary<,>))
                        {
                            mLogger.Log(methodName, string.Format("Getting data for Key: {0}", dpParent.Key), 3);
                            foreach (KeyValuePair<string, dynamic> dpChild in dpParent.Value)
                            {
                                mLogger.Log(methodName, string.Format("Key: {0}     Value: {1}", dpChild.Key, dpChild.Value), 3);

                                if (dpParent.Key == "location")
                                {
                                    PropertyInfo pi = loc.GetType().GetProperty(dpChild.Key);
                                    ValidateClassProperty(pi, dpChild.Key, "SchoolLocation");
                                    pi.SetValue(loc, Convert.ChangeType(dpChild.Value, pi.PropertyType), null);
                                }
                                else if (dpParent.Key == "principal")
                                {
                                    PropertyInfo pi = principal.GetType().GetProperty(dpChild.Key);
                                    ValidateClassProperty(pi, dpChild.Key, "Principal");
                                    pi.SetValue(principal, Convert.ChangeType(dpChild.Value, pi.PropertyType), null);
                                }
                            }
                        }
                    }
                    else if (valueType.Name == "String")
                    {
                        mLogger.Log(methodName, string.Format("Key: {0}     Value: {1}", dpParent.Key, dpParent.Value), 3);

                        PropertyInfo pi = school.GetType().GetProperty(dpParent.Key);
                        ValidateClassProperty(pi, dpParent.Key, "School");
                        pi.SetValue(school, Convert.ChangeType(dpParent.Value, pi.PropertyType), null);
                    }
                }
            }
            //  check whether we are instantiating for event data and if so, set event_type and id
            if (when_id.Item1 == "current_attributes")
            {
                school.event_type = when_id.Item1;
            }
            else if (when_id.Item1 == "previous_attributes")
            {
                school.event_type = when_id.Item1;
                school.id = when_id.Item2;
            }

            school.location = loc;
            school.principal = principal;
            mWrappedData.Schools.Add(school);

            return success;
        }

        bool InstantiateTeacher(dynamic dataDict, Tuple<string, string> when_id)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            mLogger.Log(methodName, "", 3);

            bool success = true;

            Teacher teacher = new Teacher();
            Credentials credentials = new Credentials();
            PersonName pName = new PersonName();
            Google_apps gApps = new Google_apps();

            // Get each dp (datapoint) found in dataDict
            foreach (KeyValuePair<string, dynamic> dpParent in dataDict)
            {
                if (dpParent.Value != null)
                {
                    Type valueType = dpParent.Value.GetType();
                    if (valueType.IsGenericType)
                    {
                        // This value item is not just another string value but (probably) another dictionary
                        Type baseType = valueType.GetGenericTypeDefinition();
                        if (baseType == typeof(Dictionary<,>))
                        {
                            mLogger.Log(methodName, string.Format("Getting data for Key: {0}", dpParent.Key), 3);
                            foreach (KeyValuePair<string, dynamic> dpChild in dpParent.Value)
                            {
                                mLogger.Log(methodName, string.Format("Key: {0}     Value: {1}", dpChild.Key, dpChild.Value), 3);

                                if (dpParent.Key == "credentials")
                                {
                                    PropertyInfo pi = credentials.GetType().GetProperty(dpChild.Key);
                                    ValidateClassProperty(pi, dpChild.Key, "Credentials");
                                    pi.SetValue(credentials, Convert.ChangeType(dpChild.Value, pi.PropertyType), null);
                                }
                                else if (dpParent.Key == "name")
                                {
                                    PropertyInfo pi = pName.GetType().GetProperty(dpChild.Key);
                                    ValidateClassProperty(pi, dpChild.Key, "PersonName");
                                    pi.SetValue(pName, Convert.ChangeType(dpChild.Value, pi.PropertyType), null);
                                }
                                else if (dpParent.Key == "google_apps")
                                {
                                    PropertyInfo pi = gApps.GetType().GetProperty(dpChild.Key);
                                    ValidateClassProperty(pi, dpChild.Key, "Google_apps");
                                    pi.SetValue(gApps, Convert.ChangeType(dpChild.Value, pi.PropertyType), null);
                                }
                            }
                        }
                    }
                    else if (valueType.Name == "String")
                    {
                        mLogger.Log(methodName, string.Format("Key: {0}     Value: {1}", dpParent.Key, dpParent.Value), 3);

                        PropertyInfo pi = teacher.GetType().GetProperty(dpParent.Key);
                        ValidateClassProperty(pi, dpParent.Key, "Teacher");
                        pi.SetValue(teacher, Convert.ChangeType(dpParent.Value, pi.PropertyType), null);
                    }
                }
            }
            //  check whether we are instantiating for event data and if so, set event_type and id
            if (when_id.Item1 == "current_attributes")
            {
                teacher.event_type = when_id.Item1;
            }
            else if (when_id.Item1 == "previous_attributes")
            {
                teacher.event_type = when_id.Item1;
                teacher.id = when_id.Item2;
            }

            teacher.credentials = credentials;
            teacher.name = pName;
            teacher.google_apps = gApps;
            mWrappedData.Teachers.Add(teacher);

            return success;
        }

        bool InstantiateSection(dynamic dataDict, Tuple<string, string> when_id)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            mLogger.Log(methodName, "", 3);

            bool success = true;

            Section section = new Section();
            SectionTerm term = new SectionTerm();
            List<string> studentList = new List<string>();

            // Get each dp (datapoint) found in dataDict
            foreach (KeyValuePair<string, dynamic> dpParent in dataDict) 
            {
                if (dpParent.Value != null)
                {
                    Type valueType = dpParent.Value.GetType();
                    if (valueType.IsGenericType)
                    {
                        // This value item is not just another string value but (probably) another dictionary
                        Type baseType = valueType.GetGenericTypeDefinition();
                        if (baseType == typeof(Dictionary<,>))
                        {
                            mLogger.Log(methodName, string.Format("Getting data for Key: {0}", dpParent.Key), 3);
                            foreach (KeyValuePair<string, dynamic> dpChild in dpParent.Value)
                            {
                                mLogger.Log(methodName, string.Format("Key: {0}     Value: {1}", dpChild.Key, dpChild.Value), 3);

                                if (dpParent.Key == "term")
                                {
                                    PropertyInfo pi = term.GetType().GetProperty(dpChild.Key);
                                    ValidateClassProperty(pi, dpChild.Key, "SectionTerm");
                                    pi.SetValue(term, Convert.ChangeType(dpChild.Value, pi.PropertyType), null);
                                }
                            }
                        }
                    }
                    else if (valueType == typeof(ArrayList))
                    {
                        if (dpParent.Key == "students")
                        {
                            foreach (string student in dpParent.Value)
                            {
                                studentList.Add(student);
                            }
                        }
                    }
                    else if (valueType.Name == "String")
                    {
                        mLogger.Log(methodName, string.Format("Key: {0}     Value: {1}", dpParent.Key, dpParent.Value), 3);

                        PropertyInfo pi = section.GetType().GetProperty(dpParent.Key);
                        ValidateClassProperty(pi, dpParent.Key, "Section");
                        pi.SetValue(section, Convert.ChangeType(dpParent.Value, pi.PropertyType), null);
                    }
                }
            }
            //  check whether we are instantiating for event data and if so, set event_type and id
            if (when_id.Item1 == "current_attributes")
            {
                section.event_type = when_id.Item1;
            }
            else if (when_id.Item1 == "previous_attributes")
            {
                section.event_type = when_id.Item1;
                section.id = when_id.Item2;
            }

            section.term = term;
            section.students = studentList;
            mWrappedData.Sections.Add(section);

            return success;
        }

        bool InstantiateStudent(dynamic dataDict, Tuple<string, string> when_id)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            mLogger.Log(methodName, "", 3);

            bool success = true;

            Student student = new Student();
            Credentials credentials = new Credentials();
            PersonName pName = new PersonName();
            StudentLocation stuLoc = new StudentLocation();
            Google_apps gApps = new Google_apps();

            // Get each dp (datapoint) found in dataDict
            foreach (KeyValuePair<string, dynamic> dpParent in dataDict)
            {
                if (dpParent.Value != null)
                {
                    Type valueType = dpParent.Value.GetType();
                    if (valueType.IsGenericType)
                    {
                        // This value item is not just another string value but (probably) another dictionary 
                        Type baseType = valueType.GetGenericTypeDefinition();
                        if (baseType == typeof(Dictionary<,>))
                        {
                            mLogger.Log(methodName, string.Format("Getting data for Key: {0}", dpParent.Key), 3);
                            foreach (KeyValuePair<string, dynamic> dpChild in dpParent.Value)
                            {
                                mLogger.Log(methodName, string.Format("Key: {0}::     Value: {1}", dpChild.Key, dpChild.Value), 3);

                                if (dpParent.Key == "credentials")
                                {
                                    PropertyInfo pi = credentials.GetType().GetProperty(dpChild.Key);
                                    ValidateClassProperty(pi, dpChild.Key, "Credentials");
                                    pi.SetValue(credentials, Convert.ChangeType(dpChild.Value, pi.PropertyType), null);
                                }
                                else if (dpParent.Key == "name")
                                {
                                    PropertyInfo pi = pName.GetType().GetProperty(dpChild.Key);
                                    ValidateClassProperty(pi, dpChild.Key, "PersonName");
                                    pi.SetValue(pName, Convert.ChangeType(dpChild.Value, pi.PropertyType), null);
                                }
                                else if (dpParent.Key == "location")
                                {
                                    PropertyInfo pi = stuLoc.GetType().GetProperty(dpChild.Key);
                                    ValidateClassProperty(pi, dpChild.Key, "StudentLocation");
                                    pi.SetValue(stuLoc, Convert.ChangeType(dpChild.Value, pi.PropertyType), null);
                                }
                                else if (dpParent.Key == "google_apps")
                                {
                                    PropertyInfo pi = gApps.GetType().GetProperty(dpChild.Key);
                                    ValidateClassProperty(pi, dpChild.Key, "Google_apps");
                                    pi.SetValue(gApps, Convert.ChangeType(dpChild.Value, pi.PropertyType), null);
                                }
                            }
                        }
                    }
                    else if (valueType.Name == "String")
                    {
                        mLogger.Log(methodName, string.Format("Key: {0}     Value: {1}", dpParent.Key, dpParent.Value), 3);

                        PropertyInfo pi = student.GetType().GetProperty(dpParent.Key);
                        ValidateClassProperty(pi, dpParent.Key, "Student");
                        pi.SetValue(student, Convert.ChangeType(dpParent.Value, pi.PropertyType), null);
                    }
                }
            }
            //  check whether we are instantiating for event data and if so, set event_type and id
            if (when_id.Item1 == "current_attributes")
            {
                student.event_type = when_id.Item1;
            }
            else if (when_id.Item1 == "previous_attributes")
            {
                student.event_type = when_id.Item1;
                student.id = when_id.Item2;
            }

            student.location = stuLoc;
            student.credentials = credentials;
            student.name = pName;
            student.google_apps = gApps;
            mWrappedData.Students.Add(student);

            return success;
        }

        bool InstantiateContact(dynamic dataDict)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            mLogger.Log(methodName, "", 3);

            bool success = true;

            Contact contact = new Contact();

            // Get each dp (datapoint) found in dataDict
            foreach (KeyValuePair<string, dynamic> dpParent in dataDict)
            {
                if (dpParent.Value != null)
                {
                    Type valueType = dpParent.Value.GetType();
                    if (valueType.Name == "String")
                    {
                        mLogger.Log(methodName, string.Format("Key: {0}     Value: {1}", dpParent.Key, dpParent.Value), 3);

                        PropertyInfo pi = contact.GetType().GetProperty(dpParent.Key);
                        ValidateClassProperty(pi, dpParent.Key, "Contact");
                        pi.SetValue(contact, Convert.ChangeType(dpParent.Value, pi.PropertyType), null);
                    }
                    else
                    {
                        mLogger.Log(methodName, string.Format("ALERT !  ValueType not expected: {0}", valueType.FullName), 1);
                    }
                }
            }

            mWrappedData.Contacts.Add(contact);

            return success;
        }

        bool InstantiateAdmin(dynamic dataDict)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            mLogger.Log(methodName, "", 3);

            bool success = true;

            Admin admin = new Admin();

            // Get each dp (datapoint) found in dataDict
            foreach (KeyValuePair<string, dynamic> dpParent in dataDict)
            {
                if (dpParent.Value != null)
                {
                    Type valueType = dpParent.Value.GetType();
                    if (valueType.Name == "String")
                    {
                        mLogger.Log(methodName, string.Format("Key: {0}     Value: {1}", dpParent.Key, dpParent.Value), 3);

                        PropertyInfo pi = admin.GetType().GetProperty(dpParent.Key);
                        ValidateClassProperty(pi, dpParent.Key, "Admin");
                        pi.SetValue(admin, Convert.ChangeType(dpParent.Value, pi.PropertyType), null);
                    }
                    else
                    {
                        mLogger.Log(methodName, string.Format("ALERT !  ValueType not expected: {0}", valueType.FullName), 1);
                    }
                }
            }

            mWrappedData.Admins.Add(admin);

            return success;
        }

        void GetEventData(dynamic dataDict)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            mLogger.Log(methodName, "", 3);

            string eventType = string.Empty;
            
            // Get each dp (datapoint) found in dataDict
            foreach (KeyValuePair<string, dynamic> dpParent in dataDict)
            {
                string objectId = string.Empty;
                if (dpParent.Key == "type" && dpParent.Value.GetType() == typeof(String))
                {
                    eventType = dpParent.Value;
                    mLogger.Log(methodName, string.Format("Found event type: {0}", eventType), 3);
                    continue;
                }
                if (dpParent.Key == "data")
                {
                    foreach (KeyValuePair<string, dynamic> dpChild in dpParent.Value)
                    {
                        // First we set the tuple (current vs previous data and object id) for the object that event data represents
                        Tuple<string, string> when_id = new Tuple<string, string>("","");
                        if (dpChild.Key == "object")
                        {
                            // we are looking at current data and includes all attributes/properties for that class
                            objectId = dpChild.Value["id"];
                            when_id = new Tuple<string, string>("current_attributes", "");
                        }
                        else if (dpChild.Key == "previous_attributes")
                        {
                            // we are looking at the data that was changed - previous attribute/property values only
                            when_id = new Tuple<string, string>("previous_attributes", objectId);
                        }
                        // Now instantiate the object that event data represents
                        if (eventType == "districs.updated")
                        {
                            InstantiateDistrict(dpChild.Value, when_id);
                        }
                        else if (eventType == "schools.updated")
                        {
                            InstantiateSchool(dpChild.Value, when_id);
                        }
                        else if (eventType == "teachers.updated")
                        {
                            InstantiateTeacher(dpChild.Value, when_id);
                        }
                        else if (eventType == "sections.updated")
                        {
                            InstantiateSection(dpChild.Value, when_id);
                        }
                        else if (eventType == "students.updated")
                        {
                            InstantiateStudent(dpChild.Value, when_id);
                        }
                        else
                        {
                            mLogger.Log(methodName, string.Format("ALERT !  Unexpected condition. eventType: {0}", eventType), 1);
                        }
                    }
                }
            }

            return;
        }

        /// <summary>
        /// This will validate that the CleverApiWrapper class will contain the Clever object property contained in the Clever message.
        /// If the property doesn't exist (or has a  misspell or wrong case), pi will be null and an exception will be thrown.
        /// </summary>
        /// <param name="pi"></param>
        /// <param name="cleverObjProperty"></param>
        /// <param name="cleverObj"></param>
        void ValidateClassProperty(PropertyInfo pi, string cleverObjProperty, string cleverObj)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            if (pi == null)
            {
                string errMsg = string.Format("The Object class property \"{0}\" in the Clever message was not found in the CleverApiWrapper {1} class.", cleverObjProperty, cleverObj);
                mLogger.Log(methodName, string.Format("ERROR !!  {0}", errMsg), 1);
                throw new System.ApplicationException(errMsg);
            }
        }
    }
}
