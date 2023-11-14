namespace Ilumisoft.ScriptTemplates
{
    using UnityEngine;

    public static class DebugUtils
    {
        /// <summary>
        /// Logs an error with the given message when the given object is null
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        public static void NullErrorMessage(Object obj, string message)
        {
            if(obj == null)
            {
                Debug.LogError(message);
            }
        }
    }
}