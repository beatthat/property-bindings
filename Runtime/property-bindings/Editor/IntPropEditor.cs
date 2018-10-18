using UnityEditor;
using UnityEngine;
using System;

namespace BeatThat.Properties{
	[CustomEditor(typeof(IntProp), true)]
	[CanEditMultipleObjects]
	public class IntPropEditor : ValuePropertyEditor<IntProp, int>
	{
		override public void OnInspectorGUI() 
		{
			var prop = this.target as IntProp;
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
