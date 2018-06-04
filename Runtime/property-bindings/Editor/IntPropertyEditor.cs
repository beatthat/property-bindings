using UnityEditor;
using UnityEngine;
using System;

namespace BeatThat
{
	[CustomEditor(typeof(IntProperty), true)]
	[CanEditMultipleObjects]
	public class IntPropertyEditor : ValuePropertyEditor<IntProperty, int>
	{
		override public void OnInspectorGUI() 
		{
			var prop = this.target as IntProperty;
			var valBefore = prop.value;
			EditorGUILayout.LabelField ("Value", (this.target as HasValue).valueObj + "");
			base.OnInspectorGUI();
			if (Application.isPlaying) {
				if (valBefore != prop.value) {
					prop.SetValue (prop.value, PropertyEventOptions.Force);
				}
			}
		}
	}
}