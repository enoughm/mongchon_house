namespace Ilumisoft.ScriptTemplates
{
    using UnityEditor;
    using UnityEngine.Events;

    public class AssetPostProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            TemplateManager templateManager = TemplateManager.Find();

            if (templateManager != null)
            {
                string templateDirectory = templateManager.GetPath(TemplateManager.Directory.ScriptTemplates);

                bool hasTemplatesChanged = false;

                UnityAction<string> checkTemplateChange = (path) =>
                {
                    if (path.Contains(templateDirectory))
                    {
                        hasTemplatesChanged = true;
                    }
                };

                foreach (string str in deletedAssets)
                {
                    checkTemplateChange.Invoke(str);
                }

                foreach (string str in importedAssets)
                {
                    checkTemplateChange.Invoke(str);
                }

                for (int i = 0; i < movedAssets.Length; i++)
                {
                    checkTemplateChange.Invoke(movedAssets[i]);
                    checkTemplateChange.Invoke(movedFromAssetPaths[i]);
                }

                if (hasTemplatesChanged)
                {
                    MenuItemScriptFactory.CreateMenuItemScript();
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}