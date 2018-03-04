using UnityEditor;
using UnityEngine;

namespace BeatThat
{
	public static class PropertyBindingEditor 
	{
		public static void ShowBoundPropertyField<PropertyType, ValueType>(IHasProperty<PropertyType> pb)
			where PropertyType : class
		{
			if (pb.property != null) {
				EditorGUILayout.LabelField ("Target Property", pb.property.GetType ().Name);
			} 
			else {
				EditorGUILayout.HelpBox ("Property is null or not a component", MessageType.Warning);
			}
		}
	}
}