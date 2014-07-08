using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleverApiWrapper
{
    internal class HelperClass
    {
        List<string> mValidIncludeItems = new List<string>() { "districts", "teachers", "schools", "students", "events", "sections" };
        List<string> mValidKvpKeyItems = new List<string>() { "limit", "page", "where", "sort", "count", "distinct", "grade", "created_since", "include", "id", "key" };

        //internal bool ValidateQualifierItem(Logger logger, string itemToValidate, string valType)
        //{
        //    string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //    bool success = false;

        //    if (valType == "str")
        //    {
        //        foreach (string validIncludeItem in mValidIncludeItems)
        //        {
        //            if (validIncludeItem == itemToValidate) return true;
        //        }
        //    }
        //    else if (valType == "kvp")
        //    {
        //        foreach (string validIncludeItem in mValidIncludeItems)
        //        {
        //            if (validIncludeItem == itemToValidate) return true;
        //        }
        //    }

        //    string msg = string.Format("Error !! Qualifier item: \"{0}\" is INVALID.", itemToValidate);
        //    logger.Log(methodName, msg, 1, true);

        //    return success;
        //}

        //internal string GetIncludeStringAndValidate(Logger logger, List<string> stringList)
        //{
        //    string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //    string includes = string.Empty;
        //    foreach (string inclItem in stringList)
        //    {
        //        if (!ValidateQualifierItem(logger, inclItem, "str"))
        //        {
        //            // todo - throw an error or just discard invalid item ???
        //            //continue;
        //            return "";
        //        }

        //        if (string.IsNullOrEmpty(includes))
        //        {
        //            includes = inclItem;
        //        }
        //        else
        //        {
        //            includes = string.Format("{0},{1}", includes, inclItem);
        //        }
        //    }
        //    return includes;
        //}

        /// <summary>
        /// Takes passed in argument which is a list of keyValuePairs and first validates the values against what is currently acceptable and then secondly 
        ///   convert that list of kvp's into a string that can be integrated into our URL we are building.  Additionally, if any of the keyValuePairs have key 
        ///   equal to 'id' we set that separately and returns it as part of the return Tuple.  The Tuple will have 2 strings and a bool with the first sring 
        ///   being the primary kvp string and the second Tuple component containing the id value if it exists - empty string being the default value.
        ///   The bool is simply a flag indicating that there was a problem validating at least one item.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="kvpList"></param>
        /// <returns></returns>
        internal Tuple<string, string, bool> ConvertKvpToStringAndValidate(Logger logger, List<KeyValuePair<String, String>> kvpList)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            bool success = true;
            string kvpStr = string.Empty;
            string kvpId = string.Empty;
            Tuple<string, string, bool> kvpTuple = new Tuple<string,string, bool>("","", false);
            foreach (KeyValuePair<String, String> pair in kvpList)
            {
                string kvpKey = pair.Key.ToLower();
                //  validate each key item in kvp
                if (!mValidKvpKeyItems.Contains(kvpKey))
                {
                    string msg = string.Format("Error !! Clever parameter item: \"{0}\" is INVALID.", kvpKey);
                    logger.Log(methodName, msg, 1, true);
                    return kvpTuple;
                }
                if (kvpKey == "include")
                {
                    // validate each include item  (ie: what Clever refers to as "second-level endpoint")
                    if (string.IsNullOrEmpty(pair.Value.Trim()))
                    {
                        string msg = string.Format("Error !! Clever 'include' value (second-level endpoints) is empty.");
                        logger.Log(methodName, msg, 1, true);
                        return kvpTuple;
                    }
                    string[] inclParts = pair.Value.Split(',');
                    foreach (string inclPart in inclParts)
                    {
                        if (!mValidIncludeItems.Contains(inclPart))
                        {
                            string msg = string.Format("Error !! Clever include item (second-level endpoint): \"{0}\" is INVALID.", inclPart);
                            logger.Log(methodName, msg, 1, true);
                            return kvpTuple;
                        }
                    }
                }
                // check for 'id'  and if so, set that value for the 2nd item in Tuple
                if (kvpKey == "id")
                {
                    if (!string.IsNullOrEmpty(kvpId))
                    {
                        string msg = string.Format("Unexpected Error !! kvp key = id, but id has already been set.");
                        logger.Log(methodName, msg, 1, true);
                        return kvpTuple;
                    }
                    kvpId = pair.Value;
                    continue;
                }

                // Validation is successful, so now save the kvp into our string object
                if (string.IsNullOrEmpty(kvpStr))
                {
                    kvpStr = string.Format("{0}={1}", kvpKey, pair.Value);
                }
                else
                {
                    kvpStr = string.Format("{0}&{1}={2}", kvpStr, kvpKey, pair.Value);
                }
            }
            Tuple<string, string, bool> kvpTupleFinal = new Tuple<string, string, bool>(kvpStr, kvpId, success);
            return kvpTupleFinal;
        }

        internal int ConvertStringToInt(string stringVal)
        {
            int testValue;
            int.TryParse(stringVal, out testValue);
            return testValue;
        }
    }



}
