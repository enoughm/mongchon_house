namespace Ilumisoft.ScriptTemplates.Internal.Extensions
{
    using UnityEditor;

    public static class Extensions
    {
        /// <summary>
        /// Returns the path of the folder (with "/" at the end)
        /// </summary>
        /// <param name="defaultAsset"></param>
        /// <returns></returns>
        public static string GetPath(this DefaultAsset defaultAsset)
        {
            return AssetDatabase.GetAssetPath(defaultAsset)+"/";
        }
    }
}