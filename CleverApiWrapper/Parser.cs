using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;  // for JavaScriptSerializer  & had to add ref  system.web.extensions
using System.Collections;
using System.Reflection;

namespace CleverApiWrapper
{
    class Parser
    {
        Logger mLogger;
        WrappedData mWrappedData;
        //List<District> mDistricts;
        List<School> mSchools;
        //List<Teacher> mTeachers;
        //List<Student> mStudents;
        //List<Section> mSections;
        //List<Event> mEvents;

        internal Parser(Logger logger)
        {
            mLogger = logger;
            WrappedData mWrappedData = new WrappedData();
            mWrappedData.Schools = new List<School>();
            //mDistricts = new List<District>();
            mSchools = new List<School>();
            //mTeachers = new List<Teacher>();
            //mStudents = new List<Student>();
            //mSections = new List<Section>();
            //mEvents = new List<Event>();
        }

        internal WrappedData ReadJsonDict(string msg)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            var jss = new JavaScriptSerializer();
            var masterDict = jss.Deserialize<Dictionary<string, dynamic>>(msg);
            foreach (KeyValuePair<string, dynamic> kvp in masterDict)
            {
                if (kvp.Key == "data")
                {
                    Console.WriteLine("key: {0}", kvp.Key);
                    foreach (KeyValuePair<string, dynamic> kvp2 in kvp.Value)
                    {
                        //if (kvp2.Key == "schools" || kvp2.Key == "teachers" || kvp2.Key == "sections" || kvp2.Key == "students" || kvp2.Key == "events")
                        if (kvp2.Key == "schools")
                        {
                            string targetObjectType = kvp2.Key;
                            mLogger.Log(methodName, string.Format("Found object data for: {0}", targetObjectType), 2, true);
                            foreach (KeyValuePair<string, dynamic> kvp3 in kvp2.Value)
                            {
                                mLogger.Log(methodName, string.Format("Looking for key=data.  This key: {0}", kvp3.Key), 3);
                                if (kvp3.Key == "data")
                                {
                                    ArrayList targetObjectList = new ArrayList();
                                    targetObjectList = kvp3.Value;
                                    foreach (var item in targetObjectList)
                                    {
                                        bool isDict = false;
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

                                            if (isDict)
                                            {
                                                Dictionary<string, dynamic> targetDataDict = (Dictionary<string, dynamic>)item;
                                                foreach (KeyValuePair<string, dynamic> targetData in targetDataDict)
                                                {
                                                    if (targetData.Key == "data")
                                                    {
                                                        if (targetObjectType == "schools")
                                                        {
                                                            bool instantiateSuccess = InstantiateSchool(targetObjectType, targetData.Value);
                                                        }
                                                        else if (targetObjectType == "teachers")
                                                        {
                                                            bool instantiateSuccess = InstantiateTeacher(targetObjectType, targetData.Value);
                                                        }
                                                        else if (targetObjectType == "sections")
                                                        {
                                                            bool instantiateSuccess = InstantiateStudent(targetObjectType, targetData.Value);
                                                        }
                                                        else if (targetObjectType == "students")
                                                        {
                                                            bool instantiateSuccess = InstantiateStudent(targetObjectType, targetData.Value);
                                                        }
                                                    }
                                                }

                                                // http://www.tomasvera.com/programming/using-javascriptserializer-to-parse-json-objects/
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            mWrappedData.Schools = mSchools;

            mLogger.Log(methodName, string.Format("Completed processing to parse raw Clever message."), 2);
            mLogger.Log(methodName, string.Format("Found {0} districts.", mWrappedData.Districts.Count), 2);
            mLogger.Log(methodName, string.Format("Found {0} schools.", mWrappedData.Schools.Count), 2);
            mLogger.Log(methodName, string.Format("Found {0} teachers.", mWrappedData.Teachers.Count), 2);
            mLogger.Log(methodName, string.Format("Found {0} sections.", mWrappedData.Sections.Count), 2);
            mLogger.Log(methodName, string.Format("Found {0} students.", mWrappedData.Students.Count), 2);
            return mWrappedData;
        }

        bool InstantiateSchool(string classType, dynamic dataDict)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Console.WriteLine("");
            mLogger.Log(methodName, "", 3);

            bool success = true;

            School school = new School();
            SchoolLocation loc = new SchoolLocation();
            Principal principal = new Principal();

            foreach (KeyValuePair<string, dynamic> dpParent in dataDict)
            {
                if (dpParent.Value != null)
                {
                    Type valueType = dpParent.Value.GetType();
                    if (valueType.IsGenericType)
                    {
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
                                    pi.SetValue(loc, Convert.ChangeType(dpChild.Value, pi.PropertyType), null);
                                }
                                else if (dpParent.Key == "principal")
                                {
                                    PropertyInfo pi = principal.GetType().GetProperty(dpChild.Key);
                                    pi.SetValue(principal, Convert.ChangeType(dpChild.Value, pi.PropertyType), null);
                                }
                            }
                        }
                    }
                    else if (valueType.Name == "String")
                    {
                        mLogger.Log(methodName, string.Format("Key: {0}     Value: {1}", dpParent.Key, dpParent.Value), 3);

                        PropertyInfo pi = school.GetType().GetProperty(dpParent.Key);
                        pi.SetValue(school, Convert.ChangeType(dpParent.Value, pi.PropertyType), null);
                    }
                }
            }
            school.location = loc;
            school.principal = principal;
            mSchools.Add(school);
            //mWrappedData.Schools.Add(school);

            return success;
        }

        bool InstantiateTeacher(string classType, dynamic dataDict)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            mLogger.Log(methodName, "", 3);

            bool success = true;

            Teacher teacher = new Teacher();
            Credentials credentials = new Credentials();
            PersonName pName = new PersonName();
            Google_apps gApps = new Google_apps();

            foreach (KeyValuePair<string, dynamic> dpParent in dataDict)
            {
                if (dpParent.Value != null)
                {
                    Type valueType = dpParent.Value.GetType();
                    if (valueType.IsGenericType)
                    {
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
                                    pi.SetValue(credentials, Convert.ChangeType(dpChild.Value, pi.PropertyType), null);
                                }
                                else if (dpParent.Key == "name")
                                {
                                    PropertyInfo pi = pName.GetType().GetProperty(dpChild.Key);
                                    pi.SetValue(pName, Convert.ChangeType(dpChild.Value, pi.PropertyType), null);
                                }
                                else if (dpParent.Key == "google_apps")
                                {
                                    PropertyInfo pi = gApps.GetType().GetProperty(dpChild.Key);
                                    pi.SetValue(gApps, Convert.ChangeType(dpChild.Value, pi.PropertyType), null);
                                }
                            }
                        }
                    }
                    else if (valueType.Name == "String")
                    {
                        mLogger.Log(methodName, string.Format("Key: {0}     Value: {1}", dpParent.Key, dpParent.Value), 3);

                        PropertyInfo pi = teacher.GetType().GetProperty(dpParent.Key);
                        pi.SetValue(teacher, Convert.ChangeType(dpParent.Value, pi.PropertyType), null);
                    }
                }
            }
            teacher.credentials = credentials;
            teacher.name = pName;
            teacher.google_apps = gApps;
            //mTeachers.Add(teacher);
            mWrappedData.Teachers.Add(teacher);

            return success;
        }

        bool InstantiateSection(string classType, dynamic dataDict)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            mLogger.Log(methodName, "", 3);

            bool success = true;

            Section section = new Section();

            foreach (KeyValuePair<string, dynamic> dpParent in dataDict)
            {
                if (dpParent.Value != null)
                {
                    Type valueType = dpParent.Value.GetType();
                    if (valueType.IsGenericType)
                    {
                        // condition not expected
                    }
                    else if (valueType.Name == "String")
                    {
                        mLogger.Log(methodName, string.Format("Key: {0}     Value: {1}", dpParent.Key, dpParent.Value), 3);

                        PropertyInfo pi = section.GetType().GetProperty(dpParent.Key);
                        pi.SetValue(section, Convert.ChangeType(dpParent.Value, pi.PropertyType), null);
                    }
                }
            }
            //mSections.Add(section);
            mWrappedData.Sections.Add(section);

            return success;
        }

        bool InstantiateStudent(string classType, dynamic dataDict)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            mLogger.Log(methodName, "", 3);

            bool success = true;

            Student student = new Student();
            Credentials credentials = new Credentials();
            PersonName pName = new PersonName();
            StudentLocation stuLoc = new StudentLocation();
            Google_apps gApps = new Google_apps();

            foreach (KeyValuePair<string, dynamic> dpParent in dataDict)  // dp - datapoint
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
                                    pi.SetValue(credentials, Convert.ChangeType(dpChild.Value, pi.PropertyType), null);
                                }
                                else if (dpParent.Key == "name")
                                {
                                    PropertyInfo pi = pName.GetType().GetProperty(dpChild.Key);
                                    pi.SetValue(pName, Convert.ChangeType(dpChild.Value, pi.PropertyType), null);
                                }
                                else if (dpParent.Key == "location")
                                {
                                    PropertyInfo pi = stuLoc.GetType().GetProperty(dpChild.Key);
                                    pi.SetValue(stuLoc, Convert.ChangeType(dpChild.Value, pi.PropertyType), null);
                                }
                                else if (dpParent.Key == "google_apps")
                                {
                                    PropertyInfo pi = gApps.GetType().GetProperty(dpChild.Key);
                                    pi.SetValue(gApps, Convert.ChangeType(dpChild.Value, pi.PropertyType), null);
                                }
                            }
                        }
                    }
                    else if (valueType.Name == "String")
                    {
                        mLogger.Log(methodName, string.Format("Key: {0}     Value: {1}", dpParent.Key, dpParent.Value), 3);

                        PropertyInfo pi = student.GetType().GetProperty(dpParent.Key);
                        pi.SetValue(student, Convert.ChangeType(dpParent.Value, pi.PropertyType), null);
                    }
                }
            }
            student.location = stuLoc;
            student.credentials = credentials;
            student.name = pName;
            student.google_apps = gApps;
            //mStudents.Add(student);
            mWrappedData.Students.Add(student);

            return success;
        }
    }
}
