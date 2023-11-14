namespace Ilumisoft.ScriptTemplates
{
    using UnityEditor;

    public static class ScriptFactory
    {
        public static void CreateScriptFromTemplateAsset(string templatePath, string templateAssetName)
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, $"New{templateAssetName}.cs");
        }
    }
}