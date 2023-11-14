namespace Ilumisoft.ScriptTemplates
{
    using System.Collections;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public static class AssetDatabaseUtils
    {
        /// <summary>
        /// Returns the first instance of an Asset with the given type.
        /// Returns null, if none is found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Find<T>() where T : Object
        {
            var guids = AssetDatabase.FindAssets("t: "+typeof(T).FullName);

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                T result = AssetDatabase.LoadAssetAtPath<T>(path);

                return result;
            }

            return null;
        }

        public static T[] GetAtPath<T>()
        {
            var templateManager = TemplateManager.Find();

            string path = templateManager.GetPath(TemplateManager.Directory.ScriptTemplates).RemovePrefix("Assets/");

            var arrayList = new ArrayList();
            string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);

            foreach (string fileName in fileEntries)
            {
                int index = fileName.LastIndexOf("/");
                string localPath = "Assets/" + path;

                if (index > 0)
                {
                    localPath += fileName.Substring(index);
                }

                Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

                if (t != null)
                {
                    arrayList.Add(t);
                }
            }
            
            T[] result = new T[arrayList.Count];
            
            for (int i = 0; i < arrayList.Count; i++)
            {
                result[i] = (T)arrayList[i];
            }

            return result;
        }

        private static string RemovePrefix(this string text, string prefix)
        {
            if (text.StartsWith(prefix))
            {
                text = text.Remove(0, prefix.Length);
            }

            return text;
        }
    }
}