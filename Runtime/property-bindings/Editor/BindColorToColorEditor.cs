using UnityEditor;
using UnityEngine;

namespace BeatThat.Properties{
	[CustomEditor(typeof(BindColorToColor), true)]
	[CanEditMultipleObjects]
	public class BindColorToColorEditor : PropertyBindingEditor
	{
		override public void OnInspectorGUI() 
		{
			EditorGUI.BeginChangeCheck();

			var driverProp = this.serializedObject.FindProperty("m_driver");
			EditorGUILayout.PropertyField(driverProp, new GUIContent("Driver", "Source property for the color"));

			var userAssetForDefaultProp = this.serializedObject.FindProperty("m_useAssetForDefaultValue");

			if(userAssetForDefaultProp.boolValue) {
				var assetProp = this.serializedObject.FindProperty("m_defaultValueAsset");
				EditorGUILayout.PropertyField(assetProp, new GUIContent("Color", "a shared ColorAsset for places where you want consistency"));
			}
			else {
				var defaultColorProp = this.serializedObject.FindProperty("m_defaultValue");
				EditorGUILayout.PropertyField(defaultColorProp);
			}

			EditorGUILayout.PropertyField(userAssetForDefaultProp,
				new GUIContent("Use Asset For Default", "For default color, use a shared color asset or a local color property?"));

			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_disableAutoSync"));
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_logWarningOnNoDriver"));
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_debug"));


			PropertyBindingEditor.HandleAssignProperty<HasColor, Color> (this.serializedObject, this.target);

			this.serializedObject.ApplyModifiedProperties();

			if(EditorGUI.EndChangeCheck()) {
				(this.target as BindColorToColor).UpdateProperty();
			}
		}
	}
}
