using UnityEditor;
using UnityEngine;
using System;

namespace BeatThat.Properties{
	[CustomEditor(typeof(HasValue), true)]
	[CanEditMultipleObjects]
	public class HasValueEditor : UnityEditor.Editor
	{
		override public void OnInspectorGUI() 
		{
			EditorGUILayout.LabelField ("Value", (this.target as HasValue).valueObj + "");
			base.OnInspectorGUI();
		}
	}
}
