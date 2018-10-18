using UnityEditor;

namespace BeatThat.Properties{
	[CustomEditor(typeof(DisplaysFloat), true)]
	[CanEditMultipleObjects]
    public class DisplaysFloatEditor : FloatPropEditor
	{
		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck ();

			OnDisplaysFloatInspectorGUI();

			if (EditorGUI.EndChangeCheck ()) {
				(this.target as DisplaysFloat).UpdateDisplay();
			}
		}

		virtual protected void OnDisplaysFloatInspectorGUI() 
		{
			base.OnInspectorGUI();
		}

		protected void DrawDisplaysFloatProperties(bool displayValueAsSlider = true, float valueLeft = 0f, float valueRight = 1f)
		{
			var valueProp = this.serializedObject.FindProperty("m_value");
			var debugProp = this.serializedObject.FindProperty("m_debug");
			var updateDisplayOnEnableProp = this.serializedObject.FindProperty("m_updateDisplayOnEnable");
			var applyChangesOnLateUpdate = this.serializedObject.FindProperty("m_applyChangesOnLateUpdate");

			if (displayValueAsSlider) {
				EditorGUILayout.Slider (valueProp, valueLeft, valueRight);
			} else {
				EditorGUILayout.PropertyField (valueProp);
			}
			EditorGUILayout.PropertyField(debugProp);
			EditorGUILayout.PropertyField(updateDisplayOnEnableProp);
			EditorGUILayout.PropertyField(applyChangesOnLateUpdate);

			EditPropertyBindings (this.serializedObject, this.target.GetType());
		}
	}
}
