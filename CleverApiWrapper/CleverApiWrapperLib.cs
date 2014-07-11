using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace CleverApiWrapper
{

    /// <summary>
    /// This class is the main controller for calls to this library.  It contains a DataRequest method and 2 overloaded methods as the primary library calls.  
    /// These primary methods construct a url based on passed in arguments and calls GetRawDataFromClever() to obtain a raw JSON message from Clever containing 
    ///  the SIS data. We then instantiate the Parser class to parse the JSON message and instantiate the WrappedData object that is returned to the calling code.
    ///  The WrappedData class is a simple class that contains lists of Districts, Schools, Teachers, Sections, and Students.
    /// </summary>
    public class CleverApiWrapperLib
    {
        Logger mLogger = null;
        HelperClass mHelper = new HelperClass();
        public string mCleverToken = "Bearer DEMO_TOKEN";

        public enum cleverRequestType
        {
            district,
            school,
            teacher,
            section,
            student,
            Event,
        };

        public enum cleverRequestSubType
        {
            district,
            school,
            teacher,
            section,
            student,
            grade_level,
            contact,
            admin,
            Event,
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

            string cleverObjType = requestType.ToString().ToLower() + "s";  // toLower() used to handle Events
            WrappedData wrappedData = new WrappedData();

            string url = string.Format(@"https://api.clever.com/v1.1/{0}s", requestType.ToString().ToLower());  // toLower() used to handle Events
            mLogger.Log(methodName, "url: " + url, 2);

            // Send built URl to Clever and hopefully, we get a message containing the expected data
            string rawData = GetRawDataFromClever(url);

            if (string.IsNullOrEmpty(rawData))
            {
                mLogger.Log(methodName, "Unexpected Error !! The message from Clever appears to be empty or was not properly extracted from the stream.", 1);
                throw new System.ApplicationException("The message from Clever appears to be empty or was not properly extracted from the stream.");
            }
            // The parser will translate the JSON msg and generate the Clever objects (Students, Teachers, ect) all contained in the parent object - wrappedData
            Parser parser = new Parser(mLogger);
            wrappedData = parser.ParseJsonMsg(rawData, cleverObjType);

            return wrappedData;
        }

        public WrappedData DataRequest(cleverRequestType requestType, List<KeyValuePair<String, String>> kvpList)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //// Example for "https://api.clever.com/v1.1/districts/4fd43cc56d11340000000005?include=schools,teachers" ////
            //// Example for "https://api.clever.com/v1.1/students?limit=120"                                          ////
            //// Example for "https://api.clever.com/v1.1/districts/4fd43cc56d11340000000005"                          ////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name + "_2";
            string cleverObjType = string.Empty;
            string url = string.Empty;
            string msg = string.Format("reqType: {0} :: List<string> count: {1}", requestType, kvpList.Count);
            mLogger.Log(methodName, msg, 1);

            WrappedData wrappedData = new WrappedData();

            Tuple<string, string, bool> kvpStrTuple = mHelper.ConvertKvpToStringAndValidate(mLogger, kvpList);
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
                cleverObjType = requestType.ToString().ToLower() + "s";
            }

            mLogger.Log(methodName, string.Format("Successfully validated list of KeyValuePairs and converted to a string: {0}", kvpStr), 1);

            if (string.IsNullOrEmpty(id))
            {
                url = string.Format(@"https://api.clever.com/v1.1/{0}s?{1}", requestType.ToString().ToLower(), kvpStr);
            }
            else
            {
                if (string.IsNullOrEmpty(kvpStr))
                {
                    url = string.Format(@"https://api.clever.com/v1.1/{0}s/{1}", requestType.ToString().ToLower(), id);
                }
                else
                {
                    url = string.Format(@"https://api.clever.com/v1.1/{0}s/{1}?{2}", requestType.ToString().ToLower(), id, kvpStr);
                }
            }
            mLogger.Log(methodName, "url: " + url, 2);

            // Send built URl to Clever and hopefully, we get a message containing the expected data
            string rawData = GetRawDataFromClever(url);

            if (string.IsNullOrEmpty(rawData))
            {
                mLogger.Log(methodName, "Unexpected Error !! The message from Clever appears to be empty or was not properly extracted from the stream.", 1);
                throw new System.ApplicationException("The message from Clever appears to be empty or was not properly extracted from the stream.");
            }
            // The parser will translate the JSON msg and generate the Clever objects (Students, Teachers, ect) all contained in the parent object - wrappedData
            Parser parser = new Parser(mLogger);
            wrappedData = parser.ParseJsonMsg(rawData, cleverObjType);

            return wrappedData;
        }

        public WrappedData DataRequest(cleverRequestType requestType, cleverRequestSubType requestSubType, List<KeyValuePair<String, String>> kvpList)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name + "_3";

            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            //// Example for "https://api.clever.com/v1.1/schools/530e595026403103360ff9fd/students?limit=120" ////
            //// Example for "https://api.clever.com/v1.1/schools/530e595026403103360ff9fd/students"           ////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////

            //string requestSubTypeStr = requestSubType.ToString().ToLower();
            string msg = string.Format("reqType: {0} :: reqSubType: {1}  :: List<kvp> count: {2}", requestType, requestSubType, kvpList.Count);
            mLogger.Log(methodName, msg, 1);

            WrappedData wrappedData = new WrappedData();

            string pluralItems = "s";
            string url = string.Empty;

            Tuple<string, string, bool> kvpStrTuple = mHelper.ConvertKvpToStringAndValidate(mLogger, kvpList);
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
                throw new System.ArgumentException("The Clever 'include' item (second-level endpoint) is NOT valid when using request SubTypes.");
            }
            // The assumption is that if we have an argument for cleverRequestSubType, then there WILL be an 'id' as part of the kvp's
            if (string.IsNullOrEmpty(id))
            {
                mLogger.Log(methodName, "Error !! The list argument (Clever parameter items) does NOT contain 'id'. The 'id' must be included when using cleverRequestSubType.", 1);
                throw new System.ArgumentException(string.Format("The list argument (Clever parameter items) does NOT contain 'id'. The 'id' must be included when using cleverRequestSubType."));
            }
            mLogger.Log(methodName, string.Format("Successfully validated list of KeyValuePairs and converted to a string: {0}", kvpStr), 1);

            // need to look at reqSubkey and determine what conditions where we don't need to append an "s"
            if (requestSubType == cleverRequestSubType.district
                || (requestType == cleverRequestType.teacher && requestSubType == cleverRequestSubType.school)
                || (requestType == cleverRequestType.student && requestSubType == cleverRequestSubType.school)
                || (requestType == cleverRequestType.section && requestSubType == cleverRequestSubType.school)
                || (requestType == cleverRequestType.section && requestSubType == cleverRequestSubType.teacher))
            {
                pluralItems = "";
            }

            // need to identify what type of object we will look for in parser
            string targetObjectType = requestSubType.ToString().ToLower() + "s";

            if (string.IsNullOrEmpty(kvpStr))
            {
                url = string.Format(@"https://api.clever.com/v1.1/{0}s/{1}/{2}{3}", requestType, id, requestSubType.ToString().ToLower(), pluralItems);
            }
            else
            {
                url = string.Format(@"https://api.clever.com/v1.1/{0}s/{1}/{2}{3}?{4}", requestType, id, requestSubType.ToString().ToLower(), pluralItems, kvpStr);
            }
            mLogger.Log(methodName, "url: " + url, 2);

            // Send built URl to Clever and hopefully, we get a message containing the expected data
            string rawData = GetRawDataFromClever(url);

            if (string.IsNullOrEmpty(rawData))
            {
                string errMsg = "The message from Clever appears to be empty or was not properly extracted from the stream. It may also indicate a network connection error.";
                mLogger.Log(methodName, "Unexpected Error !! " + errMsg, 1);
                throw new System.ApplicationException(errMsg);
            }
            // The parser will translate the JSON msg and generate the Clever objects (Students, Teachers, ect) all contained in the parent object - wrappedData
            Parser parser = new Parser(mLogger);
            wrappedData = parser.ParseJsonMsg(rawData, targetObjectType);

            return wrappedData;
        }

        /// <summary>
        /// We pass in a constructed url and using a WebRequest, we query Clever.com for data and expect a raw message (Json format) back.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string GetRawDataFromClever(string url)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string CleverRawMsg = string.Empty;
            try
            {
                var request = WebRequest.Create(url);
                request.Headers["Authorization"] = mCleverToken;
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
            return CleverRawMsg;
        }
    }
}
