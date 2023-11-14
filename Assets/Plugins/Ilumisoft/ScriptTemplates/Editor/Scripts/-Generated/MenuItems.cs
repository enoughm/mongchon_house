namespace Ilumisoft.ScriptTemplates
{
	using UnityEngine;
	using UnityEditor;

	public class TemplateMenuItems : MonoBehaviour 
	{
		[MenuItem("Assets/C# Script Template/MonoBehaviour", priority = 1)]
		static void MenuItem0()
		{
			ScriptFactory.CreateScriptFromTemplateAsset("Assets/Plugins/Ilumisoft/ScriptTemplates/Editor/Templates/Scripts/MonoBehaviour.txt","MonoBehaviour");
		}
		
		[MenuItem("Assets/C# Script Template/UI_LayerViewTemplate", priority = 1)]
		static void MenuItem1()
		{
			ScriptFactory.CreateScriptFromTemplateAsset("Assets/Plugins/Ilumisoft/ScriptTemplates/Editor/Templates/Scripts/UI_LayerViewTemplate.txt","UI_LayerViewTemplate");
		}
		
		[MenuItem("Assets/C# Script Template/UI_PopupTemplate", priority = 1)]
		static void MenuItem2()
		{
			ScriptFactory.CreateScriptFromTemplateAsset("Assets/Plugins/Ilumisoft/ScriptTemplates/Editor/Templates/Scripts/UI_PopupTemplate.txt","UI_PopupTemplate");
		}
		
		[MenuItem("Assets/C# Script Template/UI_RootViewTemplate", priority = 1)]
		static void MenuItem3()
		{
			ScriptFactory.CreateScriptFromTemplateAsset("Assets/Plugins/Ilumisoft/ScriptTemplates/Editor/Templates/Scripts/UI_RootViewTemplate.txt","UI_RootViewTemplate");
		}

	}
}