using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace CleverApiWrapper
{

    /// <summary>
    /// This class is the controller for calls to this library.
    /// </summary>
    public class CleverApiWrapperLib
    {
        Logger mLogger = null;
        //WrappedData mWrappedData = null;
        HelperClass mHelper = new HelperClass();
        public string mCleverToken = "Bearer DEMO_TOKEN";

        public enum cleverRequestType
        {
            district,
            school,
            teacher,
            section,
            student,
        };

        public enum cleverRequestSubType
        {
            district,
            school,
            teacher,
            section,
            student,
            grade_level,
            //properties,
        };

        public CleverApiWrapperLib()
        {
            mLogger = new Logger();
        }

        public WrappedData DataRequest(cleverRequestType requestType)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name + "_1";
            //////////////////////////////////////////////////////////////
            //// Example for "https://api.clever.com/v1.1/students"   ////
            //////////////////////////////////////////////////////////////

            string msg = string.Format("reqType: {0}", requestType);
            mLogger.Log(methodName, msg, 1);

            string cleverObjType = requestType.ToString() + "s";
            WrappedData wrappedData = new WrappedData();

            string url = string.Format(@"https://api.clever.com/v1.1/{0}s", requestType);
            mLogger.Log(methodName, "url: " + url, 2);

            // Send built URl to Clever and hopefully, we get a message containing the expected data
            string rawData = GetRawDataFromClever(url);
            //string rawData = GetTestMsg();  //                   REMOVE THIS LINE AFTER TESTING - THIS BYPASSES NETWORK CONN.!!!!!!!!!!!!!!

            if (string.IsNullOrEmpty(rawData))
            {
                mLogger.Log(methodName, "Unexpected Error !! The message from Clever appears to be empty or was not properly extracted from the stream.", 1);
                throw new System.ApplicationException("The message from Clever appears to be empty or was not properly extracted from the stream.");
            }
            // The parser will parse the JSON msg and generate the Clever objects (Students, Teachers, ect) all contained in the parent object - wrappedData
            Parser parser = new Parser(mLogger);
            wrappedData = parser.ParseJsonMsg(rawData, cleverObjType);

            return wrappedData;
        }

        //public WrappedData DataRequest(cleverRequestType requestType, List<KeyValuePair<String, String>> kvpList)            //done/////////////////////////////////////////////
        //{
        //    string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name + "_2";

        //    //////////////////////////////////////////////////////////////////////
        //    //// Example for "https://api.clever.com/v1.1/students?limit=120" ////
        //    //////////////////////////////////////////////////////////////////////


        //    //
        //    // if kvp key is "id" or "key" strip those out and then send remainder to be stringitized and validated
        //    //


        //    string msg = string.Format("reqType: {0} :: List<kvp> count: {2}", requestType, kvpList.Count);
        //    mLogger.Log(methodName, msg, 1);

        //    WrappedData wrappedData = new WrappedData();

        //    // do some initial argument validation
        //    if (kvpList.Count == 0)
        //    {
        //        mLogger.Log(methodName, "Error !! The list argument is empty.", 1);
        //        throw new System.ArgumentException("The list argument is empty. This method expects a list of keyValuePairs and containing at least one pair.");
        //    }

        //    string kvpStr = mHelper.ConvertKvpToStringAndValidate(mLogger, kvpList);  // if kvpStr string is empty, we assume an error has occurred during validation             TEST THIS FUNCTIONALITY !!!!!!!!!!!!!!! also test mulitple parameters
        //    if (string.IsNullOrEmpty(kvpStr))
        //    {
        //        throw new System.ArgumentException(string.Format("The list argument contains at least one invalid value. For more details, see log at: {0}", mLogger.mLogFile));
        //    }
        //    mLogger.Log(methodName, string.Format("Successfully validated list of KeyValuePairs and converted to a string: {0}", kvpStr), 1);

        //    string url = string.Format(@"https://api.clever.com/v1.1/{0}s/{1}?{2}", requestType, kvpStr);
        //    mLogger.Log(methodName, "url: " + url, 2);

        //    // Send built URl to Clever and hopefully, we get a message containing the expected data
        //    string rawData = GetRawDataFromClever(url);

        //    if (string.IsNullOrEmpty(rawData))
        //    {
        //        mLogger.Log(methodName, "Unexpected Error !! The message from Clever appears to be empty or was not properly extracted from the stream.", 1);
        //        throw new System.ApplicationException("The message from Clever appears to be empty or was not properly extracted from the stream.");
        //    }
        //    // The parser will parse the JSON msg and generate the Clever objects (Students, Teachers, ect) all contained in the parent object - wrappedData
        //    Parser parser = new Parser(mLogger);
        //    wrappedData = parser.ParseJsonMsg(rawData);

        //    return wrappedData;
        //}

        public WrappedData DataRequest(cleverRequestType requestType, List<KeyValuePair<String, String>> kvpList)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //// Example for "https://api.clever.com/v1.1/districts/4fd43cc56d11340000000005?include=schools,teachers" ////       tested  https://api.clever.com/v1.1/districts/4fd43cc56d11340000000005?include=schools,teachers
            //// Example for "https://api.clever.com/v1.1/students?limit=120"                                          ////
            //// Example for "https://api.clever.com/v1.1/districts/4fd43cc56d11340000000005"                          ////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name + "_2";
            string cleverObjType = string.Empty;
            string url = string.Empty;
            string msg = string.Format("reqType: {0} :: List<string> count: {1}", requestType, kvpList.Count);
            mLogger.Log(methodName, msg, 1);

            WrappedData wrappedData = new WrappedData();

            Tuple<string, string, bool> kvpStrTuple = mHelper.ConvertKvpToStringAndValidate(mLogger, kvpList);  // if kvpStr string is empty, we assume an error has occurred during validation             TEST THIS FUNCTIONALITY !!!!!!!!!!!!!!! also test mulitple parameters
            string kvpStr = kvpStrTuple.Item1;
            string id = kvpStrTuple.Item2;
            bool stringifySuccess = kvpStrTuple.Item3;

            //
            // Final Validation
            //

            if (!stringifySuccess)
            {
                mLogger.Log(methodName, "Error !! The list argument (Clever parameter items) contains at least one invalid value.", 1);
                throw new System.ArgumentException(string.Format("The list argument (Clever parameter items) contains at least one invalid value. For more details, see log at: {0}", mLogger.mLogFile));
            }

            if (kvpList.Count < 1)
            {
                mLogger.Log(methodName, "Error !! The list argument (Clever parameter items) contains NO items.", 1);
                throw new System.ArgumentException(string.Format("The list argument (Clever parameter items) contains NO items."));
            }
            if (string.IsNullOrEmpty(id))
            {
                if (kvpStr.Contains("include="))
                {
                    mLogger.Log(methodName, "Error !! The 'id' value is empty and user attempted to use Clever include item (second-level endpoint).  There must be an 'id' when using 'include'.", 1);
                    throw new System.ArgumentException("The 'id' value is empty and user attempted to use Clever include item (second-level endpoint).  There must be an 'id' when using 'include'.");
                }
            }
            if (!kvpStr.Contains("include="))
            {
                // We know the return object should be reqType, so pass that along in case it is needed
                cleverObjType = requestType.ToString() + "s";
            }

            mLogger.Log(methodName, string.Format("Successfully validated list of KeyValuePairs and converted to a string: {0}", kvpStr), 1);

            if (string.IsNullOrEmpty(id))
            {
                url = string.Format(@"https://api.clever.com/v1.1/{0}s?{1}", requestType, kvpStr);//                               TEST THIS METHOD WITH AND WITHOUT HAVING AN ID
            }
            else
            {
                if (string.IsNullOrEmpty(kvpStr))
                {
                    url = string.Format(@"https://api.clever.com/v1.1/{0}s/{1}", requestType, id);
                }
                else
                {
                    url = string.Format(@"https://api.clever.com/v1.1/{0}s/{1}?{2}", requestType, id, kvpStr);
                }
            }
            mLogger.Log(methodName, "url: " + url, 2);

            // Send built URl to Clever and hopefully, we get a message containing the expected data
            string rawData = GetRawDataFromClever(url);
            //string rawData = GetTestMsg();  //                   REMOVE THIS LINE AFTER TESTING - THIS BYPASSES NETWORK CONN.!!!!!!!!!!!!!!

            if (string.IsNullOrEmpty(rawData))
            {
                mLogger.Log(methodName, "Unexpected Error !! The message from Clever appears to be empty or was not properly extracted from the stream.", 1);
                throw new System.ApplicationException("The message from Clever appears to be empty or was not properly extracted from the stream.");
            }
            // The parser will parse the JSON msg and generate the Clever objects (Students, Teachers, ect) all contained in the parent object - wrappedData
            Parser parser = new Parser(mLogger);
            wrappedData = parser.ParseJsonMsg(rawData, cleverObjType);

            return wrappedData;
        }

        public WrappedData DataRequest(cleverRequestType requestType, cleverRequestSubType requestSubType, List<KeyValuePair<String, String>> kvpList)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name + "_3";

            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            //// Example for "https://api.clever.com/v1.1/schools/530e595026403103360ff9fd/students?limit=120" ////                   THIS HAS NOT BEEN TESTED  !!!!!!!!!!!!!!!!!!!!!!!!!!!
            //// Example for "https://api.clever.com/v1.1/schools/530e595026403103360ff9fd/students"           ////                   TESTED  https://api.clever.com/v1.1/districts/4fd43cc56d11340000000005/teachers
            ///////////////////////////////////////////////////////////////////////////////////////////////////////

            string msg = string.Format("reqType: {0} :: reqSubType: {1}  :: List<kvp> count: {2}", requestType, requestSubType, kvpList.Count);
            mLogger.Log(methodName, msg, 1);

            WrappedData wrappedData = new WrappedData();

            string pluralItems = "s";
            string url = string.Empty;

            Tuple<string, string, bool> kvpStrTuple = mHelper.ConvertKvpToStringAndValidate(mLogger, kvpList);  // if kvpStr string is empty, we assume an error has occurred during validation             TEST THIS FUNCTIONALITY !!!!!!!!!!!!!!! also test mulitple parameters
            string kvpStr = kvpStrTuple.Item1;
            string id = kvpStrTuple.Item2;
            bool stringifySuccess = kvpStrTuple.Item3;

            //
            // Final Validation
            //

            if (!stringifySuccess)
            {
                mLogger.Log(methodName, "Error !! The list argument (Clever parameter items) contains at least one invalid value.", 1);
                throw new System.ArgumentException(string.Format("The list argument (Clever parameter items) contains at least one invalid value. For more details, see log at: {0}", mLogger.mLogFile));
            }
            if (kvpList.Count < 1)
            {
                mLogger.Log(methodName, "Error !! The list argument (Clever parameter items) contains NO items.", 1);
                throw new System.ArgumentException(string.Format("The list argument (Clever parameter items) contains NO items."));
            }
            if (kvpStr.Contains("include="))
            {
                mLogger.Log(methodName, "Error !! The Clever 'include' item (second-level endpoint) is NOT valid when using request SubTypes.", 1);
                throw new System.ArgumentException("The Clever 'include' item (second-level endpoint) is NOT valid when using request SubTypes."); //            TEST THIS FUNCTIONALITY !!!!!!!!!!!!!!!  trying to use include
            }
            // The assumption is that if we have an argument for cleverRequestSubType, then there WILL be an 'id' as part of the kvp's                             TEST THIS FUNCTIONALITY !!!!!!!!!!!!!!! of missing id
            if (string.IsNullOrEmpty(id))
            {
                mLogger.Log(methodName, "Error !! The list argument (Clever parameter items) does NOT contain 'id'. The 'id' must be included when using cleverRequestSubType.", 1);
                throw new System.ArgumentException(string.Format("The list argument (Clever parameter items) does NOT contain 'id'. The 'id' must be included when using cleverRequestSubType."));
            }
            mLogger.Log(methodName, string.Format("Successfully validated list of KeyValuePairs and converted to a string: {0}", kvpStr), 1);

            // need to look at reqSubkey and determine what conditions where we need to append an "s"
            if (requestSubType == cleverRequestSubType.district
                || (requestType == cleverRequestType.teacher && requestSubType == cleverRequestSubType.school)
                || (requestType == cleverRequestType.student && requestSubType == cleverRequestSubType.school)
                || (requestType == cleverRequestType.section && requestSubType == cleverRequestSubType.school)
                || (requestType == cleverRequestType.section && requestSubType == cleverRequestSubType.teacher))
            {
                pluralItems = "";
            }

            if (string.IsNullOrEmpty(kvpStr))
            {
                url = string.Format(@"https://api.clever.com/v1.1/{0}s/{1}/{2}{3}", requestType, id, requestSubType, pluralItems);
            }
            else
            {
                url = string.Format(@"https://api.clever.com/v1.1/{0}s/{1}/{2}{3}?{4}", requestType, id, requestSubType, pluralItems, kvpStr);
            }
            mLogger.Log(methodName, "url: " + url, 2);

            // Send built URl to Clever and hopefully, we get a message containing the expected data
            string rawData = GetRawDataFromClever(url);
            //string rawData = GetTestMsg();  //                   REMOVE THIS LINE AFTER TESTING - THIS BYPASSES NETWORK CONN.!!!!!!!!!!!!!!

            if (string.IsNullOrEmpty(rawData))
            {
                string errMsg = "The message from Clever appears to be empty or was not properly extracted from the stream. It may also indicate a network connection error.";
                mLogger.Log(methodName, "Unexpected Error !! " + errMsg, 1);
                throw new System.ApplicationException(errMsg);
            }
            // The parser will parse the JSON msg and generate the Clever objects (Students, Teachers, ect) all contained in the parent object - wrappedData
            Parser parser = new Parser(mLogger);
            wrappedData = parser.ParseJsonMsg(rawData, requestSubType.ToString()+"s");

            return wrappedData;
        }

        //public WrappedData DataRequest(cleverRequestType requestType, String id)  
        //{
        //    string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name + "_5";
        //    // Example for "https://api.clever.com/v1.1/districts/4fd43cc56d11340000000005"

        //    string msg = string.Format("reqType: {0} :: id: {1}", requestType, id);
        //    mLogger.Log(methodName, msg, 1);
        //    WrappedData wrappedData = new WrappedData();

        //    // do some initial argument validation
        //    if (string.IsNullOrEmpty(id)) // todo NEED  TO  TEST
        //    {
        //        mLogger.Log(methodName, "Error !! The 'id' value is empty.", 1);
        //        throw new System.ArgumentException("The id value is empty. This method expects the id to contain a value.");
        //    }

        //    string url = string.Format(@"https://api.clever.com/v1.1/{0}s/{1}", requestType, id);
        //    mLogger.Log(methodName, "url: " + url, 2);

        //    // Send built URl to Clever and hopefully, we get a message containing the expected data
        //    string rawData = GetRawDataFromClever(url);

        //    if (string.IsNullOrEmpty(rawData))
        //    {
        //        mLogger.Log(methodName, "Unexpected Error !! The message from Clever appears to be empty or was not properly extracted from the stream.", 1);
        //        throw new System.ApplicationException("The message from Clever appears to be empty or was not properly extracted from the stream.");
        //    }
        //    // The parser will parse the JSON msg and generate the Clever objects (Students, Teachers, ect) all contained in the parent object - wrappedData
        //    Parser parser = new Parser(mLogger);
        //    wrappedData = parser.ParseJsonMsg(rawData);

        //    return wrappedData;
        //}

        //string GetRawDataFromClever_orig(string url)
        //{
        //    string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

        //    //string sURL = "https://api.clever.com/v1.1/districts/4fd43cc56d11340000000005"; //   <--- works ok
        //    //string sURL = "https://api.clever.com/v1.1/districts/4fd43cc56d11340000000005?include=schools,teachers"; //   <--- works ok
        //    //string sURL = "https://api.clever.com/v1.1/schools/530e595026403103360ff9fd";  //   <--- works ok
        //    //string sURL = "https://api.clever.com/v1.1/students/530e5960049e75a9262cff1d";  //   <--- works ok
        //    //string sURL = "https://api.clever.com/v1.1/students?limit=120";  //   <--- works ok
        //    //string sURL = "https://api.clever.com/v1.1/students";  //   <--- works ok
        //    //string sURL = "https://api.clever.com/v1.1/districts"; //    <--- works ok
        //    //string sURL = "https://api.clever.com/v1.1/sections"; //    <--- works ok
        //    //string sURL = "https://api.clever.com/v1.1/schools";  //     <--- works ok
        //    //string sURL = "https://api.clever.com/v1.1/push/events";  //     <--- works ok

        //    //    /v1.1/students?page=2
        //    //
        //    string CleverRawMsg = string.Empty;
        //    WebRequest wrGETURL = WebRequest.Create(url);

        //    wrGETURL.Headers["Authorization"] = mCleverToken;

        //    try
        //    {
        //        Stream objStream;
        //        objStream = wrGETURL.GetResponse().GetResponseStream();
        //        if (objStream == null)
        //        {
        //            int xcv = 5;
        //        }
        //        StreamReader objReader = new StreamReader(objStream);

        //        string line = "";
        //        while (line != null)
        //        {
        //            line = objReader.ReadLine();
        //            if (line != null)
        //            {
        //                CleverRawMsg = line;
        //                string dateStamp = DateTime.Now.ToString(@"MMMdd_HHmm");
        //                //SaveData(line, dateStamp);
        //                break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //
        //    }
        //    return CleverRawMsg;
        //}

        string GetRawDataFromClever(string url)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string CleverRawMsg = string.Empty;
            try
            {
                var request = WebRequest.Create(url);
                request.Headers["Authorization"] = mCleverToken;  // test the response with this line commented out
                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        StreamReader objReader = new StreamReader(responseStream);
                        string line = "";
                        while (line != null)
                        {
                            line = objReader.ReadLine();
                            if (line != null)
                            {
                                mLogger.Log(methodName, "Successfully retrieved raw data from Clever.", 3);
                                CleverRawMsg = line;
                                break;
                            }
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
                {
                    mLogger.Log(methodName, string.Format("Clever responded with an error !! Msg from Clever: {0}", ex.Message), 1, true);
                    throw new System.Net.WebException(string.Format("Clever responded with an error !!  Url: {0}", url), ex);
                    //var resp = (HttpWebResponse)ex.Response;
                    //if (resp.StatusCode == HttpStatusCode.NotFound)
                }
                else
                {
                    mLogger.Log(methodName, string.Format("Unexpected error !! ErrMsg: {0}", ex.Message), 1, true);
                }
            }
            //SaveData(CleverRawMsg, url); // Remove after testing complete %%%%%%%%%%%%%%%%%
            return CleverRawMsg;
        }



        // This is for testing only - Remove for production use
        //void SaveData(string msg, string url)
        //{
        //    string timestamp = DateTime.Now.ToString(@"yyMMdd_HHmmss");
        //    string fileName = string.Format("C:\\ProgramData\\Rick\\CleverRawMsg_{0}.txt", timestamp);
        //    using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
        //    {
        //        file.WriteLine("url: {0}", url);
        //        file.WriteLine("");
        //        file.WriteLine(msg);
                
        //        Console.WriteLine("\n Done");
        //    }
        //}

        // this is for testing purposes only - remove for production
        //string GetTestMsg()
        //{
        //    string line;
        //    using (StreamReader reader = new StreamReader("C:\\ProgramData\\Rick\\CleverRawMsg_140704_155151_2.txt"))
        //    {
        //        while ((line = reader.ReadLine()) != null)
        //        {
        //            break;
        //        }
        //    }
        //    return line;
        //}

    }
}
