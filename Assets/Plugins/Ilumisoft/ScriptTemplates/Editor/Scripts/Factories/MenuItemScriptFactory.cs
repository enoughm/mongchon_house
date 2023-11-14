namespace Ilumisoft.ScriptTemplates
{
    using System.IO;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEngine;

    public static class MenuItemScriptFactory
    {
        public static void CreateMenuItemScript()
        {
            TemplateManager templateManager = TemplateManager.Find();
            
            DebugUtils.NullErrorMessage(templateManager, ErrorMessages.TemplateManagerNotFound);

            string baseDirectory = templateManager.GetPath(TemplateManager.Directory.MenuItem);

            string filename = "MenuItems.cs";

            //Create the full path, the file will be written to
            string fullPath = Path.GetFullPath(Path.Combine(baseDirectory, filename));

            //Get the menuItem Script template
            TextAsset menuItemTemplate = templateManager.MenuItemTemplate;

            //Get all user script templates
            var templates = templateManager.Templates;

            string content = "";

            //Create the content
            for (int index = 0; index < templates.Count; index++)
            {
                var textAsset = templates[index];

                //Get the path to the template asset
                string templatePath = AssetDatabase.GetAssetPath(textAsset);

                //Add string content
                content += $"[MenuItem(\"Assets/C# Script Template/{textAsset.name}\", priority = 1)]\n" +
                           $"static void MenuItem{index}()\n" +
                            "{\n" +
                           $"\tScriptFactory.CreateScriptFromTemplateAsset(\"{templatePath}\",\"{textAsset.name}\");\n" +
                            "}";

                //Add new lines if not the last iteration
                if (index < templates.Count - 1)
                {
                    content += "\n\n";
                }
            }

            //Indent content by 2 tabs
            content = IndentLines(content, 2);

            //Replace template section tag with content
            content = menuItemTemplate.text.Replace("#MenuItemSection#", content);

            //Ensure consistent line endings
            content = Regex.Replace(content, @"\r\n?|\n", "\n");

            //Write MenuItem script
            File.WriteAllText(fullPath, content, new System.Text.UTF8Encoding(true));

            //Import the asset afterwards
            AssetDatabase.ImportAsset(Path.Combine(baseDirectory, filename));
        }

        /// <summary>
        /// Indents all lines of the given text string by the given level.
        /// The Identation uses tabs.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private static string IndentLines(string text, int level)
        {
            string res = "";

            using (var reader = new StringReader(text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    for (int i = 0; i < level; i++)
                    {
                        line = line.Insert(0, "\t");
                    }

                    res += line;
                    res += "\n";
                }
            }

            return res;
        }
    }
}