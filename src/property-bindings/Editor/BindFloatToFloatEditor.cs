using UnityEditor;
using UnityEngine;

namespace BeatThat
{
	[CustomEditor(typeof(BindFloatToFloat), true)]
	public class BindFloatToFloatEditor : UnityEditor.Editor
	{
		override public void OnInspectorGUI() 
		{
			base.OnInspectorGUI ();
			PropertyBindingEditor.ShowBoundPropertyField<HasFloat, float>(this.target as IHasProperty<HasFloat>);
		}
	}
}