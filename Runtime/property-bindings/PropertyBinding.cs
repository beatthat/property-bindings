using BeatThat.SafeRefs;
using BeatThat.TransformPathExt;
using BeatThat.Controllers;
using BeatThat;
using UnityEngine;
using System;
using UnityEngine.Serialization;
namespace BeatThat.Properties
{

	public enum TargetPropertyAssignmentType { FindAtRuntime = 0, AssignableType = 1, FindOnGameObject = 2 }

	/// <summary>
	/// Non-generic marker base class that exists mainly to enable things like a common default Custom Editor for property bindings
	/// </summary>
	public class PropertyBinding : Subcontroller, IHasProperty
	{
		virtual public Type propertyInterface { get { throw new NotImplementedException (); } } 

		virtual public object propertyObject { get { throw new NotImplementedException();  } }

		/// <summary>
		/// (possilby undefined) unity-assignable type for the property, e.g. if the property interface is IHasValue<float>, 
		/// then propertyTypeAssignable might be component impl HasFloat
		/// </summary>
		/// <value>The property interface.</value>
		virtual public bool GetPropertyUnityAssignableType(out Type type)  
		{
			throw new NotImplementedException (); 
		}

		/// <summary>
		/// The value type of the property IFF the property interface implements IHasValue<T> (which most should)
		/// </summary>
		/// <value>The type of the property value.</value>
		public bool GetPropertyValueType(out Type valueType) 
		{
			return this.propertyInterface.GetValueType (out valueType);
		}

	}

	/// <summary>
	/// Use a PropertyBinding when you have a property component and you want to keep it's value in sync with some external value.
	/// </summary>
	public abstract class PropertyBinding<PropertyType> : PropertyBinding<PropertyType, PropertyType>
		where PropertyType : class 
	{
	}


	/// <summary>
	/// Use a PropertyBinding when you have a property component and you want to keep it's value in sync with some external value.
	/// </summary>
	public abstract class PropertyBinding<PropertyInterface, PropertyUnityAssignableType> : PropertyBinding, IHasProperty<PropertyInterface>
		where PropertyInterface : class
		where PropertyUnityAssignableType : class, PropertyInterface
	{
		
		
		[HideInInspector][FormerlySerializedAs("m_assignProperty")]public TargetPropertyAssignmentType m_targetPropertyAssignment;
		[HideInInspector][FormerlySerializedAs("m_driven")]public PropertyUnityAssignableType m_property;
		[HideInInspector]public GameObject m_propertyGameObject;

        public TargetPropertyAssignmentType targetPropertyAssignment 
        { 
            get { return m_targetPropertyAssignment; } 
            set { m_targetPropertyAssignment = value;  } 
        }

		/// <summary>
		/// The interface for the property, e.g. IHasValue<float>.
		/// If this is in fact an interface it may be different from the PropertyTypeAssignable, 
		/// which can be a concrete Component class that implements the interface (but also can be assigned in Unity editor)
		/// </summary>
		/// <value>The property interface.</value>
		override public Type propertyInterface { get { return typeof(PropertyInterface); } }

		/// <summary>
		/// (possilby undefined) unity-assignable type for the property, e.g. if the property interface is IHasValue<float>, 
		/// then propertyTypeAssignable might be component impl HasFloat
		/// </summary>
		/// <value>The property interface.</value>
		override public bool GetPropertyUnityAssignableType(out Type type)  
		{
			type = typeof(PropertyUnityAssignableType);
			return typeof(Component).IsAssignableFrom (type);
		}

		override public object propertyObject
		{
			get
			{
				return this.property;
			}
		}

		public PropertyInterface property 
		{ 
			get { 
				var p = m_propertyResolved.value;
				if (p != null) {
					return p;
				}

				switch (m_targetPropertyAssignment) {
				case TargetPropertyAssignmentType.AssignableType:
				case TargetPropertyAssignmentType.FindAtRuntime:
					FindAndAssignProperty();
					
					if ((p = m_propertyResolved.value) == null) {
						return null;
					}
					return p;
				case TargetPropertyAssignmentType.FindOnGameObject:
					if (m_propertyGameObject == null) {
						return null;
					}
					if ((p = m_propertyGameObject.GetComponent<PropertyInterface> ()) == null) {
						return null;
					}

					m_propertyResolved = new SafeRef<PropertyInterface> (p);
					return p;
				}

				return null;
			} 
			set { 
				m_propertyResolved = new SafeRef<PropertyInterface>(value);
                m_property = value as PropertyUnityAssignableType;
			}
		}
		private SafeRef<PropertyInterface> m_propertyResolved;

		public PropertyInterface driven { get {return this.property; } }

		public object GetDrivenObject ()
		{
			return this.property;
		}

		public bool ClearDriven ()
		{
			this.property = null;
			m_property = null;
			return true;
		}
			
		virtual protected bool FindProperty(out object prop)
		{
			return PropertyBindingHelper.FindOrAddTarget(this, typeof(PropertyInterface), out prop);
		}

		protected bool FindAndAssignProperty()
		{
            if (m_targetPropertyAssignment == TargetPropertyAssignmentType.AssignableType && m_property != null)
            {
                this.property = m_property;
                return true;
            }

			object tmp;
            PropertyInterface prop;
			if(!FindProperty(out tmp) || (prop = tmp as PropertyInterface) == null) {
				return false;
			}

			this.property = prop;

            if (m_targetPropertyAssignment == TargetPropertyAssignmentType.AssignableType)
            {
                var assignable = prop as PropertyUnityAssignableType;
                if (assignable != null)
                {
                    m_property = assignable;
                }
            }

            return true;
		}

		sealed override protected void BindSubcontroller()
		{
			if(this.property == null) {
				FindAndAssignProperty ();
			}
			BindProperty();
		}

		sealed override protected void UnbindSubcontroller()
		{
			UnbindProperty();
			this.property = null;
		}

        virtual protected void BindProperty() {}
		virtual protected void UnbindProperty() {}
	}


    public interface IHasProperty
	{
		object propertyObject { get; }		
	}

	public interface IHasProperty<PropertyType> : IDrive<PropertyType>, IHasProperty
		where PropertyType : class
	{
		PropertyType property { get; }
	}

	public static class PropertyBindingHelper
	{
		public static bool FindOrAddTarget(Component binding, Type targetType, out object target, bool warnOnCannotAdd = true)
		{
			target = binding.GetComponent(targetType);
			if(target != null) {
				return true;
			}
            
			if(!typeof(Component).IsAssignableFrom(targetType)) {
#if UNITY_EDITOR || DEBUG_UNSTRIP
				if (warnOnCannotAdd)
				{
					Debug.LogWarning("[" + Time.frameCount + "][" + binding.Path()
						+ "] missing required property " + targetType.Name
						+ " and the given type is not a component (cannot add)/ Bailing out");
				}
#endif
				return false;
			}
   
			//Debug.LogWarning("[" + Time.frameCount + "][" + binding.Path() 
				//+ "] missing required property " + targetType.Name
				//+ ". Generally better to add the property explicitly. Or maybe this behaviour is not longer wanted?");

			if (targetType.IsAbstract) {
#if UNITY_EDITOR || DEBUG_UNSTRIP
				if (warnOnCannotAdd)
				{
					Debug.LogWarning("[" + Time.frameCount + "][" + binding.Path()
						+ "] missing required property " + targetType.Name
						+ " and the given type is abstract (cannot add)/ Bailing out");
				}
#endif
				return false;
			}

			target = binding.gameObject.AddComponent(targetType);

			return (target != null);
		}

		public static bool FindOrAddTarget<TargetType>(Component binding, out TargetType target) where TargetType : class
		{
			object tmp;
			FindOrAddTarget (binding, typeof(TargetType), out tmp);
			return ((target = tmp as TargetType) != null);
		}
	}



}



