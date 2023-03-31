using System.Collections;
using UnityEngine;

namespace NodeGraph.Utilities
{
    public static class Helper
    {
        /// <summary>
        /// Empty string debug check
        /// </summary>
        public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
        {
            if (string.IsNullOrEmpty(stringToCheck))
            {
                Debug.Log(fieldName + " is empty");
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Empty string debug check
        /// </summary>
        public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
        {
            foreach (var item in enumerableObjectToCheck)
            {
                if (item == null)
                {
                    Debug.Log(fieldName + "has null value in object " + thisObject.name);
                    return true;
                }
            }

            return false;
        }
    }
}
