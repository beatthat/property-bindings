using UnityEditor;
using UnityEngine;

namespace BeatThat
{
	[CustomEditor(typeof(BindIntToInt), true)]
	public class BindIntToIntEditor : UnityEditor.Editor
	{
		override public void OnInspectorGUI() 
		{
			base.OnInspectorGUI ();
			PropertyBindingEditor.ShowBoundPropertyField<HasInt, int>(this.target as IHasProperty<HasInt>);
		}
	}
}