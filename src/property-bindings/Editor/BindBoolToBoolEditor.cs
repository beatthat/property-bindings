using UnityEditor;
using UnityEngine;

namespace BeatThat
{
	[CustomEditor(typeof(BindBoolToBool), true)]
	public class BindBoolToBoolEditor : UnityEditor.Editor
	{
		override public void OnInspectorGUI() 
		{
			base.OnInspectorGUI ();
			PropertyBindingEditor.ShowBoundPropertyField<HasBool, bool>(this.target as IHasProperty<HasBool>);
		}
	}
}