namespace Ilumisoft.ScriptTemplates
{
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UIElements;
    
    internal class ScriptTemplateSettingsProvider : SettingsProvider
    {
        private ReorderableList reorderableList;
        private TextAsset[] templateAssets = new TextAsset[0];

        public ScriptTemplateSettingsProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope){ }

        private void OnProjectChanged()
        {
            this.templateAssets = Ilumisoft.ScriptTemplates.AssetDatabaseUtils.GetAtPath<TextAsset>();
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            EditorApplication.projectChanged += OnProjectChanged;

            OnProjectChanged();

            this.reorderableList =
                new ReorderableList(this.templateAssets, typeof(TextAsset), false, false, true, true);

            this.reorderableList.drawElementCallback = DrawElementCallback;
            this.reorderableList.onAddCallback += OnAddCallback;
            this.reorderableList.onRemoveCallback += OnRemoveCallback;
        }

        private void OnRemoveCallback(ReorderableList list)
        {
            var item = this.templateAssets[list.index];

            var itemlist = this.templateAssets.ToList();

            DeleteTemplate(item, onDelete: () =>
            {
                itemlist.Remove(item);

                this.templateAssets = itemlist.ToArray();

                list.list = this.templateAssets;
            });
        }

        public override void OnDeactivate()
        {
            EditorApplication.projectChanged -= OnProjectChanged;
        }

        private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            var element = this.templateAssets[index];

            Rect elementRect = new Rect(rect)
            {
                width = rect.width - 100,
            };
            
            Rect buttonRect = new Rect(rect)
            {
                x = rect.width - 40,
                y = rect.y + 1,
                width = 50,
            };

            GUIContent buttonText = new GUIContent(element.name);

            var style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleLeft;
            style.fontSize = style.fontSize;

            var icon = EditorGUIUtility.IconContent("TextAsset Icon").image as Texture2D;
            buttonText.image = icon;

            EditorGUIUtility.SetIconSize(new Vector2(14, 14));

            GUI.Label(rect, buttonText, style);

            if (GUI.Button(buttonRect, "Edit", EditorStyles.miniButton))
            {
                AssetDatabase.OpenAsset(element);
            }

            EditorGUIUtility.SetIconSize(Vector2.zero);
        }

        private void OnAddCallback(ReorderableList list)
        {
            OnImportButtonClick();
            GUIUtility.ExitGUI();
        }

        public override void OnGUI(string searchContext)
        {
            var templateManager = TemplateManager.Find();

            if(templateManager == null)
            {
                EditorGUILayout.HelpBox(ErrorMessages.TemplateManagerNotFound, MessageType.Error);
                return;
            }

            string location = templateManager.GetPath(TemplateManager.Directory.ScriptTemplates);

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(9);

                GUILayout.Label("Location: " + location);

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Show", EditorStyles.miniButton, GUILayout.Width(50)))
                {
                    OnShowTemplateFolderButtonClick();
                }

                GUILayout.Space(22);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(4);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(10);

                EditorGUILayout.BeginVertical();
                {
                    this.reorderableList.DoLayoutList();
                }
                EditorGUILayout.EndVertical();

                GUILayout.Space(10);
            }
            GUILayout.EndHorizontal();
        }

        private void DeleteTemplate(TextAsset template, UnityAction onDelete = null)
        {
            if (EditorUtility.DisplayDialog("Delete template \"" + template.name + "\"?",
                "Are you really sure you want to delete \"" + template.name + "\"? This cannot be undone.", "Yes",
                "Cancel"))
            {
                onDelete?.Invoke();

                string path = AssetDatabase.GetAssetPath(template);
                AssetDatabase.DeleteAsset(path);
            }
        }

        private void OnShowTemplateFolderButtonClick()
        {
            var templateManager = TemplateManager.Find();
            DebugUtils.NullErrorMessage(templateManager, ErrorMessages.TemplateManagerNotFound);

            string location = templateManager.GetPath(TemplateManager.Directory.ScriptTemplates);

            var folder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(location.TrimEnd('/'));

            EditorGUIUtility.PingObject(folder);
        }

        private void OnImportButtonClick()
        {
            string path = EditorUtility.OpenFilePanel("Add Script Template", "", "txt");

            if (path.Length != 0)
            {
                var templateManager = TemplateManager.Find();

                string destdir = templateManager.GetPath(TemplateManager.Directory.ScriptTemplates);

                try
                {
                    File.Copy(path, Path.Combine(destdir, Path.GetFileName(path)));
                }
                catch (IOException)
                {
                    EditorUtility.DisplayDialog("File already exists",
                        "Could not import template, because another one with the same name already exists", "Ok");
                }

                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// Registers the settings provider in the project settings
        /// </summary>
        /// <returns></returns>
        [SettingsProvider]
        public static SettingsProvider Register()
        {
            var provider = new ScriptTemplateSettingsProvider("Project/Script Templates", SettingsScope.Project);

            return provider;
        }
    }
}