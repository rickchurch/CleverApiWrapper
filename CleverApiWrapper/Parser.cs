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
        List<School> mSchools;
        List<Teacher> mTeachers;
        List<Student> mStudents;

        internal Parser(Logger logger)
        {
            mLogger = logger;
            WrappedData mWrappedData = new WrappedData();
            mSchools = new List<School>();
            mTeachers = new List<Teacher>();
            mStudents = new List<Student>();
        }

        internal void ReadJsonDict(string msg)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            var jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<Dictionary<string, dynamic>>(msg);
            foreach (KeyValuePair<string, dynamic> kvp in dict)
            {
                if (kvp.Key == "data")
                {
                    Console.WriteLine("key: {0}", kvp.Key);
                    foreach (KeyValuePair<string, dynamic> kvp2 in kvp.Value)
                    {
                        if (kvp2.Key == "schools" || kvp2.Key == "teachers" || kvp2.Key == "sections" || kvp2.Key == "students") // or teachers or students or sections or
                        {
                            Console.WriteLine(""); 
                            Console.WriteLine(""); 
                            string CleverObjectTypes = kvp2.Key;
                            Console.WriteLine("key: {0}", kvp2.Key);
                            mLogger.Log(methodName, string.Format("Found object data for: {0}", kvp2.Key), 2, true);
                            foreach (KeyValuePair<string, dynamic> kvp3 in kvp2.Value)
                            {
                                Console.WriteLine("key: {0}", kvp3.Key);
                                if (kvp3.Key == "data")
                                {
                                    ArrayList myAL = new ArrayList();
                                    myAL = kvp3.Value;
                                    foreach (var item in myAL)
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
                                                Dictionary<string, dynamic> dictB = (Dictionary<string, dynamic>)item;
                                                foreach (KeyValuePair<string, dynamic> kvp9 in dictB)
                                                {
                                                    if (kvp9.Key == "data")
                                                    {
                                                        if (CleverObjectTypes == "schools")
                                                        {
                                                            bool populateSuccess = PopulateSchool(CleverObjectTypes, kvp9.Value);
                                                        }
                                                        else
                                                        {
                                                            bool populateSuccess = PopulateObject(CleverObjectTypes, kvp9.Value);
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
        }

        bool PopulateObject(string classType, dynamic data)
        {
            // might be good method to populate class
            //  http://stackoverflow.com/questions/9167083/populate-a-class-from-a-dictionary
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Console.WriteLine("");
            mLogger.Log(methodName, "", 3);

            bool success = true;

            foreach (KeyValuePair<string, dynamic> kvp in data)
            {
                if (kvp.Value != null)
                {
                    Type valueType = kvp.Value.GetType();
                    if (valueType.IsGenericType)
                    {
                        Type baseType = valueType.GetGenericTypeDefinition();
                        if (baseType == typeof(Dictionary<,>))
                        {
                            Console.WriteLine("Getting data for Key: {0}", kvp.Key);
                            foreach (KeyValuePair<string, dynamic> kvp2 in kvp.Value)
                            {
                                Console.WriteLine("Sub Key: {0}     Value: {1}", kvp2.Key, kvp2.Value);
                                mLogger.Log(methodName, string.Format("{0}:: {1}", kvp2.Key, kvp2.Value), 3);
                            }
                        }
                    }
                    else if (valueType.Name == "String")
                    {
                        Console.WriteLine("Key: {0}     Value: {1}", kvp.Key, kvp.Value);
                        mLogger.Log(methodName, string.Format("{0}: {1}", kvp.Key, kvp.Value), 3);
                    }
                }

            }
            return success;
        }
        bool PopulateSchool(string classType, dynamic data)
        {
            // might be good method to populate class
            //  http://stackoverflow.com/questions/9167083/populate-a-class-from-a-dictionary
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Console.WriteLine("");
            mLogger.Log(methodName, "", 3);

            bool success = true;

            School school = new School();
            SchoolLocation loc = new SchoolLocation();
            Principal principal = new Principal();

            foreach (KeyValuePair<string, dynamic> kvp in data)
            {
                if (kvp.Value != null)
                {
                    Type valueType = kvp.Value.GetType();
                    if (valueType.IsGenericType)
                    {
                        Type baseType = valueType.GetGenericTypeDefinition();
                        if (baseType == typeof(Dictionary<,>))
                        {
                            Console.WriteLine("Getting data for Key: {0}", kvp.Key);
                            foreach (KeyValuePair<string, dynamic> kvp2 in kvp.Value)
                            {
                                Console.WriteLine("Sub Key: {0}     Value: {1}", kvp2.Key, kvp2.Value);
                                mLogger.Log(methodName, string.Format("{0}:: {1}", kvp2.Key, kvp2.Value), 3);

                                if (kvp.Key == "location")
                                {
                                    PropertyInfo pi = loc.GetType().GetProperty(kvp2.Key);
                                    pi.SetValue(loc, Convert.ChangeType(kvp2.Value, pi.PropertyType), null);
                                }
                                else if (kvp.Key == "principal")
                                {
                                    PropertyInfo pi = principal.GetType().GetProperty(kvp2.Key);
                                    pi.SetValue(principal, Convert.ChangeType(kvp2.Value, pi.PropertyType), null);
                                }
                            }
                        }
                    }
                    else if (valueType.Name == "String")
                    {
                        Console.WriteLine("Key: {0}     Value: {1}", kvp.Key, kvp.Value);
                        mLogger.Log(methodName, string.Format("{0}: {1}", kvp.Key, kvp.Value), 3);

                        PropertyInfo pi = school.GetType().GetProperty(kvp.Key);
                        pi.SetValue(school, Convert.ChangeType(kvp.Value, pi.PropertyType), null);
                    }
                }

            }
            school.location = loc;
            school.principal = principal;
            mSchools.Add(school);

            return success;
        }
    }
}
