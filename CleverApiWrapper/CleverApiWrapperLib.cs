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
        string mCleverToken = "Bearer DEMO_TOKEN";

        public enum cleverRequestType
        {
            district,
            school,
            teacher,
            section,
            student,
        };

        public CleverApiWrapperLib()
        {
            mLogger = new Logger();
        }

        //public WrappedData DataRequest(requestType requestType, String requestSubType)
        //{
        //    WrappedData wrappedData = new WrappedData();
        //    string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name + "_1";
        //    string msg = string.Format("reqType: {0} :: reqSubType: {1}", requestType, requestSubType);
        //    mLogger.Log(methodName, msg, 1);

        //    return wrappedData;
        //}

        //public WrappedData DataRequest(String requestType, String requestSubType, List<KeyValuePair<String, String>> kvpList)
        //{
        //    WrappedData wrappedData = new WrappedData();
        //    string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name + "_2";
        //    string msg = string.Format("reqType: {0} :: reqSubType: {1} :: List<kvp> count: {2}", requestType, requestSubType, kvpList.Count);
        //    mLogger.Log(methodName, msg, 1);

        //    return wrappedData;
        //}

        public WrappedData DataRequest(cleverRequestType requestType, String requestSubType, String id, List<String> includeList)
        {
            // TODO - do I really need the 2nd string value for anything ????
            // target method - june 14
            WrappedData wrappedData = new WrappedData();
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name + "_3";
            string msg = string.Format("reqType: {0} :: reqSubType: {1} :: id: {2} :: List<string> count: {3}", requestType, requestSubType, id, includeList.Count);
            mLogger.Log(methodName, msg, 1);

            string includes = mHelper.GetIncludeStringFromList(mLogger, includeList);

            //string sURL = "https://api.clever.com/v1.1/districts/4fd43cc56d11340000000005?include=schools,teachers"; //   <--- works ok
            string url = string.Format(@"https://api.clever.com/v1.1/{0}s/{1}?include={2}", requestType, id, includes);
            mLogger.Log(methodName, "url: " + url, 2);
            //string rawData = GetRawDataFromClever(url);
            string rawData = TestWebResponseError(url);
            Parser parser = new Parser(mLogger);
            //wrappedData = parser.ReadJsonDict(rawData); // need a better name for ReadJsonDict

            return wrappedData;
        }

        //public WrappedData DataRequest(String requestType, String requestSubType, String id, List<KeyValuePair<String, String>> kvpList)
        //{
        //    WrappedData wrappedData = new WrappedData();
        //    string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name + "_4";
        //    string msg = string.Format("reqType: {0} :: reqSubType: {1} :: id: {2} :: List<kvp> count: {3}", requestType, requestSubType, id, kvpList.Count);
        //    mLogger.Log(methodName, msg, 1);

        //    return wrappedData;
        //}

        //public WrappedData DataRequest(String requestType, String requestSubType, String id)
        //{
        //    WrappedData wrappedData = new WrappedData();
        //    string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name + "_5";
        //    string msg = string.Format("reqType: {0} :: reqSubType: {1} :: id: {2}", requestType, requestSubType, id);
        //    mLogger.Log(methodName, msg, 1);

        //    return wrappedData;
        //}

        string GetRawDataFromClever(string url)
        {
            //string sURL = "https://api.clever.com/v1.1/districts/4fd43cc56d11340000000005"; //   <--- works ok
            //string sURL = "https://api.clever.com/v1.1/districts/4fd43cc56d11340000000005?include=schools,teachers"; //   <--- works ok
            //string sURL = "https://api.clever.com/v1.1/schools/530e595026403103360ff9fd";  //   <--- works ok
            //string sURL = "https://api.clever.com/v1.1/students/530e5960049e75a9262cff1d";  //   <--- works ok
            //string sURL = "https://api.clever.com/v1.1/students?limit=120";  //   <--- works ok
            //string sURL = "https://api.clever.com/v1.1/students";  //   <--- works ok
            //string sURL = "https://api.clever.com/v1.1/districts"; //    <--- works ok
            //string sURL = "https://api.clever.com/v1.1/sections"; //    <--- works ok
            //string sURL = "https://api.clever.com/v1.1/schools";  //     <--- works ok
            //string sURL = "https://api.clever.com/v1.1/push/events";  //     <--- works ok

            //    /v1.1/students?page=2
            //
            string CleverRawMsg = string.Empty;
            WebRequest wrGETURL = WebRequest.Create(url);

            wrGETURL.Headers["Authorization"] = mCleverToken;

            try
            {
                Stream objStream;
                objStream = wrGETURL.GetResponse().GetResponseStream();
                if (objStream == null)
                {
                    int xcv = 5;
                }
                StreamReader objReader = new StreamReader(objStream);

                string line = "";
                while (line != null)
                {
                    line = objReader.ReadLine();
                    if (line != null)
                    {
                        CleverRawMsg = line;
                        string dateStamp = DateTime.Now.ToString(@"MMMdd_HHmm");
                        //SaveData(line, dateStamp);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                //
            }
            return CleverRawMsg;
        }

        string TestWebResponseError(string uri)
        {
            string CleverRawMsg = string.Empty;
            try
            {
                var request = WebRequest.Create(uri);
                request.Headers["Authorization"] = mCleverToken;  // test the response with this line commented out
                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        // Process the stream
                        //Stream objStream;
                        //objStream = wrGETURL.GetResponse().GetResponseStream();
                        StreamReader objReader = new StreamReader(responseStream);

                        string line = "";
                        while (line != null)
                        {
                            line = objReader.ReadLine();
                            if (line != null)
                            {
                                CleverRawMsg = line;
                                string dateStamp = DateTime.Now.ToString(@"MMMdd_HHmm");
                                //SaveData(line, dateStamp);
                                break;
                            }
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError &&
                    ex.Response != null)
                {
                    var resp = (HttpWebResponse)ex.Response;
                    if (resp.StatusCode == HttpStatusCode.NotFound)
                    {
                        // Do something
                    }
                    else
                    {
                        // Do something else
                    }
                }
                else
                {
                    // Do something else
                }
            }
            return CleverRawMsg;
        }




        void SaveData(string msg, string title)
        {
            string fileName = string.Format("C:\\ProgramData\\Rick\\{0}_logFile.txt", title);
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                file.WriteLine(msg);
                Console.WriteLine("\n Done");
            }
        }
    }
}
