using UnityEditor;
using UnityEngine;
using System;

namespace BeatThat.Properties{
    [CustomEditor(typeof(BoolProp), true)]
	[CanEditMultipleObjects]
    public class BoolPropEditor : ValuePropertyEditor<BoolProp, bool>
	{
		override public void OnInspectorGUI() 
		{
            var prop = this.target as BoolProp;
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
