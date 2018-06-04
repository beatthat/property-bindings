using UnityEditor;
using UnityEngine;
using System;

namespace BeatThat
{
	[CustomEditor(typeof(DateTimeProp), true)]
	[CanEditMultipleObjects]
	public class DateTimePropEditor : UnityEditor.Editor
	{
		override public void OnInspectorGUI() 
		{
			if(Application.isPlaying) {
				var prop = (this.target as DateTimeProp);
				var dateStrBefore = prop.value.ToString();
				var dateChanged = EditorGUILayout.TextField("Value:", dateStrBefore);
				if(dateChanged != dateStrBefore) {
					DateTime d;
					if(DateTime.TryParse(dateChanged, out d)) {
						prop.value = d;
					}
				}
			}
			base.OnInspectorGUI();
		}
	}
}