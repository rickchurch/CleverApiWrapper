using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleverApiWrapper
{
    public class Credentials
    {
        public string district_username { get; set; }
        public string district_password { get; set; }
    }

    public class Name
    {
        public string first { get; set; }
        public string middle { get; set; }
        public string last { get; set; }
    }

    public class Google_apps
    {
    }

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

            string msg = string.Format("Include item: {0} is INVALID.  Discarding item.", includeItem);
            logger.Log(methodName, msg, 1);

            return success;
        }

        internal string GetStringFromList(Logger logger, List<string> stringList)
        {
            string includes = string.Empty;
            foreach (string inclItem in stringList)
            {
                if (!ValidateIncludeItem(logger, inclItem))
                {
                    // todo - throw an error or just discard invalid item ???  for now just log and discard
                    continue;
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
