using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace BeatThat
{
    [CustomEditor(typeof(PropertyBinding), true)]
    public class PropertyBindingEditor : UnityEditor.Editor
    {
        virtual protected void OnEnable()
        {
            TryEnsurePropertyHasTarget();
        }

        /// <summary>
        /// Tries to ensure the property has a target by either finding one or by (guessing) 
        /// and creating a wrapper for one of the PropertyBinding's sibling components,
        /// e.g. if the PropertyBinding is PropertyBinding<HasColor> and there's a sibling component of type Image 
        /// then may wrap that image as an IHasValue<Color> with a GraphicColor component.
        /// </summary>
        private void TryEnsurePropertyHasTarget()
        {
            var tgtPropAssignmentProp = FindTargetPropertyAssignmentProp(this.serializedObject);
            if ((TargetPropertyAssignmentType)tgtPropAssignmentProp.intValue == TargetPropertyAssignmentType.FindOnGameObject)
            {
                // if configuration has been set to find the target property on some (external) GameObject, then we don't want to mess with that
                return;
            }

            Type valueType;
            if ((this.target as PropertyBinding).GetPropertyValueType(out valueType))
            {
                // if the propertyinterface targets an IHasValue<ValueType> then find/create a component that implements that IHasValue<ValueType>
                PropertyBindingEditor.HandleDrivenProperty(valueType, this.target, FindPropertyProp(this.serializedObject), false);
                this.serializedObject.ApplyModifiedProperties();
            }
        }

        override public void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var propBinding = this.target as PropertyBinding;
            var propertyInterface = propBinding.propertyInterface;

            //Debug.LogWarning("[" + Time.frameCount + "] OnInspectorGUI for " + propBinding.GetType().Name);

            Type propertyUnityAssignableType;
            propBinding.GetPropertyUnityAssignableType(out propertyUnityAssignableType);

            Type valueType;
            if (propBinding.GetPropertyValueType(out valueType))
            {
                PropertyBindingEditor.HandleAssignProperty(propertyUnityAssignableType, propertyInterface, valueType, this.serializedObject, this.target);
            }
            else
            {
                Debug.LogWarning("[" + Time.frameCount + "] unable to get value type for propertybinding " + propBinding.GetType().Name);
            }
        }

        public static void HandleAssignProperty<PropertyType, ValType>(SerializedObject so, UnityEngine.Object target)
            where PropertyType : class, IHasValue<ValType>
        {
            HandleAssignProperty<PropertyType, PropertyType, ValType>(so, target);
        }

        public static void HandleAssignProperty<PropertyType, PropertyInterface, ValType>(SerializedObject so, UnityEngine.Object target)
            where PropertyInterface : class, IHasValue<ValType>
            where PropertyType : class, PropertyInterface
        {
            HandleAssignProperty(propType: typeof(PropertyType), propInterface: typeof(PropertyInterface), valueType: typeof(ValType), so: so, target: target);
        }

        private static SerializedProperty FindPropertyProp(SerializedObject so)
        {
            return so.FindProperty("m_property");
        }

        private static SerializedProperty FindPropertyGameObjectProp(SerializedObject so)
        {
            return so.FindProperty("m_propertyGameObject");
        }

        private static SerializedProperty FindTargetPropertyAssignmentProp(SerializedObject so)
        {
            return so.FindProperty("m_targetPropertyAssignment");
        }

        public static void HandleAssignProperty(Type propType, Type propInterface, Type valueType, SerializedObject so, UnityEngine.Object target)
        {
            var tgtPropAssignmentProp = FindTargetPropertyAssignmentProp(so);
            var propertyProp = FindPropertyProp(so);
            var propertyGOProp = FindPropertyGameObjectProp(so);


            if (tgtPropAssignmentProp == null || propertyProp == null || propertyGOProp == null)
            {
                Debug.LogWarning("Expect PropertyBindings to have properties m_assignProperty,  m_property, and m_propertyGameObject");
                return;
            }

            EditorGUILayout.PropertyField(tgtPropAssignmentProp,
                new GUIContent("Target Property", "Target Property may be found at runtime or set manually in Unity Editor"));

            switch ((TargetPropertyAssignmentType)tgtPropAssignmentProp.intValue)
            {
                case TargetPropertyAssignmentType.FindAtRuntime:
                    HandleDrivenProperty(valueType, target, propertyProp, true);
                    var curTgt = propertyProp.objectReferenceValue ?? ((target as IHasProperty) != null) ? (target as IHasProperty).propertyObject : null;

                    EditorGUILayout.LabelField("Cur Target", (curTgt != null) ? curTgt.GetType().Name : "[none found]");
                    if (curTgt != null && (curTgt as Component != null))
                    {
                        EditorGUILayout.LabelField("Cur Target Path: ", (curTgt as Component).Path());
                    }
                    break;
                case TargetPropertyAssignmentType.AssignableType:
                    EditorGUILayout.PropertyField(propertyProp);
                    HandleDrivenProperty(valueType, target, propertyProp, true);
                    break;
                case TargetPropertyAssignmentType.FindOnGameObject:
                    EditorGUILayout.PropertyField(propertyGOProp);
                    break;
            }

            so.ApplyModifiedProperties();

        }

        public static void HandleDrivenProperty<ValType>(UnityEngine.Object target, SerializedProperty prop, bool showGUIOptions)
        {
            HandleDrivenProperty(typeof(ValType), target, prop, showGUIOptions);
        }

        public static void HandleDrivenProperty(Type valueType, UnityEngine.Object target, SerializedProperty prop, bool showGUIOptions)
        {
            var curTgt = (prop != null) ? prop.objectReferenceValue as Component : null;
            if (curTgt != null)
            {
                // already set leave it alone
                return;
            }

            using (var comps = ListPool<Component>.Get())
            using (var driversAddable = ListPool<TypeAndDrivenType>.Get())
            using (var drivesExisting = ListPool<Component>.Get())
            {
                (target as Component).GetSiblingComponents<Component>(comps);
                foreach (var c in comps)
                {
                    using (var wrapperTypes = ListPool<Type>.Get())
                    {
                        FindTargetPropertyWrapperTypes.FindWrapperTypes(c.GetType(), valueType, wrapperTypes);

                        foreach (var wt in wrapperTypes)
                        {

                            if (comps.Find(existingComp => wt.IsAssignableFrom(existingComp.GetType())) != null)
                            {
                                drivesExisting.Add(c);
                            }
                            else
                            {
                                driversAddable.Add(new TypeAndDrivenType
                                {
                                    type = wt,
                                    drivenType = c.GetType()
                                });
                            }
                        }
                    }
                }

                if (TargetExisting(drivesExisting, prop, showGUIOptions))
                {
                    return;
                }

                TargetAddable(target, driversAddable, prop, showGUIOptions);

            }
        }

        private static bool TargetExisting(IList<Component> drivers, SerializedProperty prop, bool showGUIOptions)
        {
            switch (drivers.Count)
            {
                case 0:
                    return false;
                case 1:
                    if (prop != null)
                    {
                        prop.objectReferenceValue = drivers[0];
                    }
                    return true;
                default:
                    if (showGUIOptions)
                    {
                        foreach (var d in drivers)
                        {
                            if (GUILayout.Button("Target driver " + d.GetType().Name))
                            {
                                if (prop != null)
                                {
                                    prop.objectReferenceValue = d;
                                }
                                return true;
                            }
                        }
                    }
                    return false;
            }
        }

        private static bool TargetAddable(UnityEngine.Object target, IList<TypeAndDrivenType> driversAddable, SerializedProperty prop, bool showGUIOptions)
        {
            switch (driversAddable.Count)
            {
                case 0:
                    return false;
                case 1:
                    var newDriver = (target as Component).AddIfMissing(driversAddable[0].type);
                    var comment = (target as Component).gameObject.AddComponent<Comment>();

                    comment.text = "Added a " + newDriver.GetType().Name + " to wrap sibling " + driversAddable[0].drivenType.Name + " as the target for " + target.GetType().Name
                        + "\n\nIf you don't want the " + newDriver.GetType().Name + " component, either delete the " + target.GetType().Name + " component or provide it with a valid target."
                        + "\n\nFeel free to delete this comment";

                    if (prop != null)
                    {
                        prop.objectReferenceValue = newDriver;
                    }
                    return true;
                default:
                    if (showGUIOptions)
                    {
                        foreach (var dt in driversAddable)
                        {
                            if (GUILayout.Button("Add an " + dt.type.Name + " (wraps " + dt.drivenType.Name + ")"))
                            {
                                var selectedDriver = (target as Component).AddIfMissing(driversAddable[0].type);
                                if (prop != null)
                                {
                                    prop.objectReferenceValue = selectedDriver;
                                }
                                return true;
                            }
                        }
                    }
                    return false;
            }
        }
    

		public static void ShowBoundPropertyField(IHasProperty pb)
		{
            if (pb.propertyObject != null)
			{
                EditorGUILayout.LabelField("Target Property", pb.propertyObject.GetType().Name);
			}
			else
			{
				EditorGUILayout.HelpBox("Target property is unassigned and cannot determine the target", MessageType.Warning);
			}
		}
	}
}