﻿using System;
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

        internal Parser(Logger logger)
        {
            mLogger = logger;
            mWrappedData = new WrappedData();
        }

        //internal WrappedData ParseJsonMsg_ORIG(string msg, string knownObject = "")
        //{
        //    string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

        //    var jss = new JavaScriptSerializer();
        //    var masterDict = jss.Deserialize<Dictionary<string, dynamic>>(msg);
        //    foreach (KeyValuePair<string, dynamic> kvp in masterDict)
        //    {
        //        if (kvp.Key == "data")
        //        {

        //            if (kvp.Value.GetType() == typeof(ArrayList))
        //            {
        //                if (string.IsNullOrEmpty(knownObject))
        //                {
        //                    mLogger.Log(methodName, string.Format("Unexpected error!!  I don't know what the Clever object type is."), 1, true);
        //                    return mWrappedData;
        //                }
        //                else
        //                {
        //                    ProcessArrayObject(kvp.Value, knownObject);
        //                }
        //                return mWrappedData;
        //            }

        //            //
        //            //  Need to allow for an array list for kvp.value
        //            //



        //            mLogger.Log(methodName, string.Format("Upper most level key: {0}", kvp.Key), 3);
        //            foreach (KeyValuePair<string, dynamic> kvp2 in kvp.Value)
        //            {
        //                if (kvp2.Key == "schools" || kvp2.Key == "teachers" || kvp2.Key == "sections" || kvp2.Key == "students" || kvp2.Key == "events")
        //                {
        //                    string targetObjectType = kvp2.Key;
        //                    mLogger.Log(methodName, string.Format("Found object data for: {0}", targetObjectType), 2, true);
        //                    foreach (KeyValuePair<string, dynamic> kvp3 in kvp2.Value)
        //                    {
        //                        mLogger.Log(methodName, string.Format("Looking for key=data.  This key: {0}", kvp3.Key), 3);
        //                        if (kvp3.Key == "data")
        //                        {
        //                            if (kvp3.Value.GetType() == typeof(ArrayList))
        //                            {
        //                                ProcessArrayObject(kvp3.Value, targetObjectType);
        //                                return mWrappedData;
        //                            }



        //                            ArrayList targetObjectList = new ArrayList();
        //                            targetObjectList = kvp3.Value;
        //                            foreach (var item in targetObjectList)
        //                            {
        //                                bool isDict = false;
        //                                if (item != null)
        //                                {
        //                                    Type valueType = item.GetType();
        //                                    if (valueType.IsGenericType)
        //                                    {
        //                                        Type baseType = valueType.GetGenericTypeDefinition();
        //                                        if (baseType == typeof(Dictionary<,>))
        //                                        {
        //                                            isDict = true;
        //                                        }
        //                                    }

        //                                    if (isDict)
        //                                    {
        //                                        Dictionary<string, dynamic> targetDataDict = (Dictionary<string, dynamic>)item;
        //                                        foreach (KeyValuePair<string, dynamic> targetData in targetDataDict)
        //                                        {
        //                                            if (targetData.Key == "data")
        //                                            {
        //                                                if (targetObjectType == "schools")
        //                                                {
        //                                                    bool instantiateSuccess = InstantiateSchool(targetObjectType, targetData.Value);
        //                                                }
        //                                                else if (targetObjectType == "teachers")
        //                                                {
        //                                                    bool instantiateSuccess = InstantiateTeacher(targetObjectType, targetData.Value);
        //                                                }
        //                                                else if (targetObjectType == "sections")
        //                                                {
        //                                                    bool instantiateSuccess = InstantiateStudent(targetObjectType, targetData.Value);
        //                                                }
        //                                                else if (targetObjectType == "students")
        //                                                {
        //                                                    bool instantiateSuccess = InstantiateStudent(targetObjectType, targetData.Value);
        //                                                }
        //                                            }
        //                                        }
        //                                    }

        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    mLogger.Log(methodName, "", 2);
        //    mLogger.Log(methodName, string.Format("Completed processing to parse raw Clever message."), 2);
        //    mLogger.Log(methodName, string.Format("Found {0} districts.", mWrappedData.Districts.Count), 2);
        //    mLogger.Log(methodName, string.Format("Found {0} schools.", mWrappedData.Schools.Count), 2);
        //    mLogger.Log(methodName, string.Format("Found {0} teachers.", mWrappedData.Teachers.Count), 2);
        //    mLogger.Log(methodName, string.Format("Found {0} sections.", mWrappedData.Sections.Count), 2);
        //    mLogger.Log(methodName, string.Format("Found {0} students.", mWrappedData.Students.Count), 2);
        //    return mWrappedData;
        //}


        internal WrappedData ParseJsonMsg(string msg, string knownObject = "")
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            var jss = new JavaScriptSerializer();
            var masterDict = jss.Deserialize<Dictionary<string, dynamic>>(msg);
            foreach (KeyValuePair<string, dynamic> kvp in masterDict)
            {
                if (kvp.Key == "data")
                {

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
                    }
                    else if (kvp.Value.GetType() == typeof(Dictionary<string, dynamic>))
                    {
                        mLogger.Log(methodName, string.Format("Putting masterDict into an array object and sending it on to be processed."), 1, true);
                        if (kvp.Key == "data")
                        {
                            ArrayList arrList = new ArrayList();
                            arrList.Add(masterDict);
                            ProcessArrayObject(arrList, knownObject);
                        }
                    }
                    else
                    {
                        mLogger.Log(methodName, string.Format("Upper most level key: {0}", kvp.Key), 3);
                        foreach (KeyValuePair<string, dynamic> kvp2 in kvp.Value)
                        {
                            if (kvp2.Key == "districts" || kvp2.Key == "schools" || kvp2.Key == "teachers" || kvp2.Key == "sections" || kvp2.Key == "students" || kvp2.Key == "events")
                            {
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

                }
            }

            mLogger.Log(methodName, "", 2);
            mLogger.Log(methodName, string.Format("Completed processing to parse raw Clever message."), 2);
            mLogger.Log(methodName, string.Format("Found {0} districts.", mWrappedData.Districts.Count), 2);
            mLogger.Log(methodName, string.Format("Found {0} schools.", mWrappedData.Schools.Count), 2);
            mLogger.Log(methodName, string.Format("Found {0} teachers.", mWrappedData.Teachers.Count), 2);
            mLogger.Log(methodName, string.Format("Found {0} sections.", mWrappedData.Sections.Count), 2);
            mLogger.Log(methodName, string.Format("Found {0} students.", mWrappedData.Students.Count), 2);
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
                    if (isDict)
                    {
                        Dictionary<string, dynamic> targetDataDict = (Dictionary<string, dynamic>)item;
                        foreach (KeyValuePair<string, dynamic> targetData in targetDataDict)
                        {
                            if (targetData.Key == "data")
                            {
                                if (targetObjectType == "districts")
                                {
                                    bool instantiateSuccess = InstantiateDistrict(targetObjectType, targetData.Value);
                                }
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
                                    bool instantiateSuccess = InstantiateSection(targetObjectType, targetData.Value);
                                }
                                else if (targetObjectType == "students")
                                {
                                    bool instantiateSuccess = InstantiateStudent(targetObjectType, targetData.Value);
                                }
                            }
                        }
                    }
                }
            }
        }

        bool InstantiateDistrict(string classType, dynamic dataDict)
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
                        // condition not expected
                        mLogger.Log(methodName, string.Format("ALERT !  Condition not expected. valueType of dpParent: {1}", valueType.FullName), 1);
                    }
                    else if (valueType.Name == "String")
                    {
                        mLogger.Log(methodName, string.Format("Key: {0}     Value: {1}", dpParent.Key, dpParent.Value), 3);

                        PropertyInfo pi = district.GetType().GetProperty(dpParent.Key);
                        ValidateClassProperty(pi, dpParent.Key, "District");
                        //if (pi == null)
                        //{
                        //    string errMsg = string.Format("The Object class property \"{0}\" in the Clever message was not found in the CleverApiWrapper District class.", dpParent.Key);
                        //    mLogger.Log(methodName, string.Format("ERROR !!  {0}", errMsg), 1);
                        //    throw new System.ApplicationException(errMsg);
                        //}
                        pi.SetValue(district, Convert.ChangeType(dpParent.Value, pi.PropertyType), null);
                    }
                }
            }
            mWrappedData.Districts.Add(district);

            return success;
        }

        bool InstantiateSchool(string classType, dynamic dataDict)
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
            school.location = loc;
            school.principal = principal;
            mWrappedData.Schools.Add(school);

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
            teacher.credentials = credentials;
            teacher.name = pName;
            teacher.google_apps = gApps;
            mWrappedData.Teachers.Add(teacher);

            return success;
        }

        bool InstantiateSection(string classType, dynamic dataDict)
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
            section.term = term;
            section.students = studentList;
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
            student.location = stuLoc;
            student.credentials = credentials;
            student.name = pName;
            student.google_apps = gApps;
            mWrappedData.Students.Add(student);

            return success;
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
