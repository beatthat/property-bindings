using System;
using UnityEditor;
using UnityEngine;

namespace BeatThat
{

    /// <summary>
	/// Editor functionality that works for [Bool|Int|Float|etc.]Property (assumes a value member variable called 'm_value')
    /// Stored in an abstract class because not allowed to have multiple types in [CustomEditor] attr
    /// </summary>
	public class ValuePropertyEditor<ParamType, ValueType> : HasValueEditor
        where ParamType : Component, IHasValue<ValueType>
	{
        public override void OnInspectorGUI()
		{
			base.OnInspectorGUI ();
			EditPropertyBindings (this.serializedObject, GetType());
		}

        public static void EditPropertyBindings(SerializedObject so, Type forType)
        {
            if (!Application.isPlaying)
            {

                bool showBindingProperty = false, showDrivenField = false, legacyCheckAnyBound = false;

                var bindOrDrivePropertyOpts = so.FindProperty("m_bindOrDrivePropertyOptions");
                if (bindOrDrivePropertyOpts != null)
                {
                    EditorGUILayout.PropertyField(bindOrDrivePropertyOpts);
                    switch ((BindOrDrivePropertyOptions)bindOrDrivePropertyOpts.intValue)
                    {
                        case BindOrDrivePropertyOptions.BindToProperty:
                            showBindingProperty = true;
                            break;
                        case BindOrDrivePropertyOptions.DriveProperty:
                            showDrivenField = true;
                            break;
                    }
                }
                else
                {
                    var enablePropertyBinding = so.FindProperty("m_enablePropertyBinding");
                    if (enablePropertyBinding != null)
                    {
                        EditorGUILayout.PropertyField(enablePropertyBinding);
                        if (enablePropertyBinding.boolValue)
                        {
                            showBindingProperty = showDrivenField = legacyCheckAnyBound = true;
                        }
                    }
                }

                var anyBound = false;

                if (showBindingProperty)
                {

                    const string bindToPropName = "m_bindToProperty";
                    var bindToProperty = so.FindProperty(bindToPropName);

                    if (bindToProperty == null)
                    {
                        Debug.LogWarning(forType.Name + " expected to find property with name " + bindToPropName);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(bindToProperty);
                        anyBound |= bindToProperty.objectReferenceValue != null;
                    }

                    if (!anyBound)
                    {
                        EditorGUILayout.HelpBox("Enable Drive Property is set, but 'bindToProperty' is unassigned", MessageType.Warning);
                    }
                }

                if (showDrivenField)
                {
                    const string drivePropName = "m_driveProperty";
                    var driveProperty = so.FindProperty(drivePropName);

                    if (driveProperty == null)
                    {
                        Debug.LogWarning(forType.Name + " expected to find property with name " + drivePropName);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(driveProperty);
                        anyBound |= driveProperty.objectReferenceValue != null;
                    }


                    if (!anyBound)
                    {
                        EditorGUILayout.HelpBox("Enable Drive Property is set, but 'driveProperty' is unassigned", MessageType.Warning);
                    }
                }

                if (legacyCheckAnyBound && !anyBound)
                {
                    EditorGUILayout.HelpBox("Enable Property Binding is set, but neither 'bindToProperty' nor 'driveProperty' is assigned", MessageType.Warning);
                }

                so.ApplyModifiedProperties();

                return;
            }
        }

        private static bool GetValue(SerializedProperty p, ref ValueType value)
        {
            if(p == null)
            {
                return false;
            }

            if(typeof(ValueType) == typeof(int))
            {
                value = (ValueType)System.Convert.ChangeType(p.intValue, typeof(ValueType));
                return true;
            }

            if (typeof(ValueType) == typeof(float))
            {
                value = (ValueType)System.Convert.ChangeType(p.floatValue, typeof(ValueType));
                return true;
            }

            if (typeof(ValueType) == typeof(bool))
            {
                value = (ValueType)System.Convert.ChangeType(p.boolValue, typeof(ValueType));
                return true;
            }

            return false;
        }
	}
}