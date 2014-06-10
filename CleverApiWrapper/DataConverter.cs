using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleverApiWrapper
{
    /// <summary>
    /// This class is the controller for calls to this library.
    /// </summary>
    public class DataConverter
    {
        Logger mLogger = null;
        //WrappedData mWrappedData = null;
   
        public DataConverter()
        {
            mLogger = new Logger();
        }

        public WrappedData DataRequest(String requestType, String requestSubType)
        {
            WrappedData wrappedData = new WrappedData();
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name + "_1";
            string msg = string.Format("reqType: {0} :: reqSubType: {1}", requestType, requestSubType);
            mLogger.Log(methodName, msg, 1);

            return wrappedData;
        }

        public WrappedData DataRequest(String requestType, String requestSubType, List<KeyValuePair<String, String>> kvpList)
        {
            WrappedData wrappedData = new WrappedData();
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name + "_2";
            string msg = string.Format("reqType: {0} :: reqSubType: {1} :: List<kvp> count: {2}", requestType, requestSubType, kvpList.Count);
            mLogger.Log(methodName, msg, 1);

            return wrappedData;
        }

        public WrappedData DataRequest(String requestType, String requestSubType, String id, List<String> includeList)
        {
            WrappedData wrappedData = new WrappedData();
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name + "_3";
            string msg = string.Format("reqType: {0} :: reqSubType: {1} :: id: {2} :: List<string> count: {3}", requestType, requestSubType, id, includeList.Count);
            mLogger.Log(methodName, msg, 1);

            return wrappedData;
        }

        public WrappedData DataRequest(String requestType, String requestSubType, String id, List<KeyValuePair<String, String>> kvpList)
        {
            WrappedData wrappedData = new WrappedData();
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name + "_4";
            string msg = string.Format("reqType: {0} :: reqSubType: {1} :: id: {2} :: List<kvp> count: {3}", requestType, requestSubType, id, kvpList.Count);
            mLogger.Log(methodName, msg, 1);

            return wrappedData;
        }

        public WrappedData DataRequest(String requestType, String requestSubType, String id)
        {
            WrappedData wrappedData = new WrappedData();
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name + "_5";
            string msg = string.Format("reqType: {0} :: reqSubType: {1} :: id: {2}", requestType, requestSubType, id);
            mLogger.Log(methodName, msg, 1);

            return wrappedData;
        }

    }
}
