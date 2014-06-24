using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleverApiWrapper
{
    internal class HelperClass
    {
        List<string> mValidIncludeItems = new List<string>() { "districts", "teachers", "schools", "students", "events", "sections" };

        internal bool ValidateIncludeItem(Logger logger, string includeItem)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            bool success = false;

            foreach (string validIncludeItem in mValidIncludeItems)
            {
                if (validIncludeItem == includeItem) return true;
            }

            string msg = string.Format("Error !! Include item: \"{0}\" is INVALID.", includeItem);
            logger.Log(methodName, msg, 1, true);

            return success;
        }

        internal string GetIncludeStringFromList(Logger logger, List<string> stringList)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string includes = string.Empty;
            foreach (string inclItem in stringList)
            {
                if (!ValidateIncludeItem(logger, inclItem))
                {
                    // todo - throw an error or just discard invalid item ???  for now just log and discard
                    //continue;
                    return "";
                }

                if (string.IsNullOrEmpty(includes))
                {
                    includes = inclItem;
                }
                else
                {
                    includes = string.Format("{0},{1}", includes, inclItem);
                }
            }
            return includes;
        }

        internal int ConvertStringToInt(string stringVal)
        {
            int testValue;
            int.TryParse(stringVal, out testValue);
            return testValue;
        }
    }



}
