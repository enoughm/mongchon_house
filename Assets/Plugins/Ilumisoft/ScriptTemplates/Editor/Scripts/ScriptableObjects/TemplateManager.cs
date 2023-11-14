namespace Ilumisoft.ScriptTemplates
{
    using Ilumisoft.ScriptTemplates.Internal.Extensions;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class TemplateManager : ScriptableObject
    {
        public enum Directory
        {
            MenuItem = 0,
            ScriptTemplates = 1
        }

        [Tooltip("The template of the MenuItems script")]
        [SerializeField]
        private TextAsset menuItemTemplate = null;

        [Tooltip("The directory the generated MenuItems script will be written to")]
        [SerializeField]
        private DefaultAsset menuItemDirectory = null;

        [Tooltip("The directory containing all script templates")]
        [SerializeField]
        private DefaultAsset scriptTemplateDirectory = null;

        public TextAsset MenuItemTemplate => this.menuItemTemplate;

        public string GetPath(Directory directory)
        {
            switch (directory)
            {
                case Directory.MenuItem:
                    return this.menuItemDirectory.GetPath();

                case Directory.ScriptTemplates:
                    return this.scriptTemplateDirectory.GetPath();

                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets a list containing all script template text assets of the project
        /// </summary>
        public List<TextAsset> Templates
        {
            get => AssetDatabaseUtils.GetAtPath<TextAsset>().ToList();
        }

        public static TemplateManager Find()
        {
            var instance = AssetDatabaseUtils.Find<TemplateManager>();

            return instance;
        }
    }
}