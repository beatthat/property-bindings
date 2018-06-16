using BeatThat.Pools;
using UnityEditor;
using UnityEngine;

namespace BeatThat.Properties{
	[CustomEditor(typeof(ProxiesFloat), true)]
	public class ProxiesFloatEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck ();

			base.OnInspectorGUI();

			this.serializedObject.Update();
			DrivenField(this);
			this.serializedObject.ApplyModifiedProperties();

			if (EditorGUI.EndChangeCheck ()) {
				(this.target as DisplaysFloat).UpdateDisplay();
			}
		}

		virtual protected void OnDisplaysFloatInspectorGUI() 
		{
			base.OnInspectorGUI();
		}

		public static void FindDrivenSiblingButton(UnityEditor.Editor editor)
		{
			var drivenProp = editor.serializedObject.FindProperty("m_driven");

			EditorGUILayout.PropertyField(drivenProp);

			EnsureDrivenNotCircular(editor);
		}

		public static void DrivenField(UnityEditor.Editor editor)
		{
			var drivenProp = editor.serializedObject.FindProperty("m_driven");

			EditorGUILayout.PropertyField(drivenProp);

			EnsureDrivenNotCircular(editor);
		}

		public static void EnsureDrivenNotCircular(UnityEditor.Editor editor)
		{
			var proxiesFloat = editor.target as ProxiesFloat;

			var drivenProp = editor.serializedObject.FindProperty("m_driven");

			var driven = drivenProp.objectReferenceValue as HasFloat;

			using(var validTargets = ListPool<HasFloat>.Get()) {
				proxiesFloat.FindAllNonCircularTargets(validTargets);

				if(driven != null && proxiesFloat.IsInDrivenChainOf(driven)) {
					drivenProp.objectReferenceValue = (validTargets.Count > 0)? validTargets[0]: null; 
				}

				if(validTargets.Count > 1) {
					var selectedIx = -1;
					using(var options = ArrayPool<GUIContent>.Get(validTargets.Count)) {
						for(int i = 0; i < options.array.Length; i++) {
							if(object.ReferenceEquals(validTargets[i], driven)) {
								selectedIx = i;
							}
							options.array[i] = new GUIContent(validTargets[i].GetType().Name);
						}

						var nextSelectedIx = EditorGUILayout.Popup(new GUIContent("Driven (select)", "there is more than one valid 'driven' target on this GameObject"), selectedIx, options.array);

						if(nextSelectedIx != selectedIx) {
							drivenProp.objectReferenceValue = (nextSelectedIx >= 0)? validTargets[nextSelectedIx]: null;
						}
					}
				}
			}
		}
	}
}

