using BeatThat;
using UnityEngine;
using System;

namespace BeatThat
{
	

	/// <summary>
	/// Use a PropertyBinding when you have a property component and you want to keep it's value in sync with some external value.
	/// </summary>
	public abstract class PropertyBinding<PropertyType> : Subcontroller, IHasProperty<PropertyType>
		where PropertyType : class
	{
		public PropertyType property { get { return m_property.value; } set { m_property = new SafeRef<PropertyType>(value); } }
		private SafeRef<PropertyType> m_property;

		virtual protected bool FindProperty(out object prop)
		{
			return PropertyBindingHelper.GetProperty(this, typeof(PropertyType), out prop);
		}

		sealed override protected void BindSubcontroller()
		{
			if(this.property == null) {
				object tmp;
				PropertyType prop;
				if(!FindProperty(out tmp) || (prop = tmp as PropertyType) == null) {
					return;
				}

				this.property = prop;
			}
			BindProperty();
		}

		sealed override protected void UnbindSubcontroller()
		{
			UnbindProperty();
			this.property = null;
		}

		abstract protected void BindProperty();
		virtual protected void UnbindProperty() {}
	}

	/// <summary>
	/// Use a PropertyBinding when you have a property component and you want to keep it's value in sync with some external value.
	/// </summary>
	public abstract class PropertyBinding<ControllerType, PropertyType> : Subcontroller<ControllerType>, IHasProperty<PropertyType>
		where ControllerType : class
		where PropertyType : class
	{
		public PropertyType property { get { return m_property.value; } private set { m_property = new SafeRef<PropertyType>(value); } }
		private SafeRef<PropertyType> m_property;

		virtual protected bool FindProperty(out object prop)
		{
			return PropertyBindingHelper.GetProperty(this, typeof(PropertyType), out prop);
		}

		sealed override protected void BindSubcontroller()
		{
			if(this.property == null) {
				object tmp;
				PropertyType prop;
				if(!FindProperty(out tmp) || (prop = tmp as PropertyType) == null) {
					return;
				}

				this.property = prop;
			}
			BindProperty();
		}

		sealed override protected void UnbindSubcontroller()
		{
			UnbindProperty();
			this.property = null;
		}

		abstract protected void BindProperty();
		virtual protected void UnbindProperty() {}
	}

	public interface IHasProperty<PropertyType>
	{
		PropertyType property { get; }
	}

	public static class PropertyBindingHelper
	{
		public static bool GetProperty(Component binding, Type propertyType, out object prop)
		{
			prop = binding.GetComponent(propertyType);
			if(prop != null) {
				return true;
			}

			if(!typeof(Component).IsAssignableFrom(propertyType)) {
				Debug.LogWarning("[" + Time.frameCount + "][" + binding.Path() 
					+ "] missing required property " + propertyType.Name
					+ " and the given type is not a component (cannot add)/ Bailing out");

				return false;
			}

			Debug.LogWarning("[" + Time.frameCount + "][" + binding.Path() 
				+ "] missing required property " + propertyType.Name
				+ ". Generally better to add the property explicitly. Or maybe this behaviour is not longer wanted?");

			prop = binding.gameObject.AddComponent(propertyType);

			return (prop != null);
		}

		public static bool GetProperty<PropertyType>(Component binding, out PropertyType prop) where PropertyType : class
		{
			object tmp;
			GetProperty (binding, typeof(PropertyType), out tmp);
			return ((prop = tmp as PropertyType) != null);
//			prop = binding.GetComponent<PropertyType>();
//			if(prop != null) {
//				return true;
//			}
//
//			if(!typeof(Component).IsAssignableFrom(typeof(PropertyType))) {
//				Debug.LogWarning("[" + Time.frameCount + "][" + binding.Path() 
//					+ "] missing required property " + typeof(PropertyType).Name
//					+ " and the given type is not a component (cannot add)/ Bailing out");
//
//				return false;
//			}
//
//			Debug.LogWarning("[" + Time.frameCount + "][" + binding.Path() 
//				+ "] missing required property " + typeof(PropertyType).Name
//				+ ". Generally better to add the property explicitly. Or maybe this behaviour is not longer wanted?");
//
//			prop = binding.gameObject.AddComponent(typeof(PropertyType)) as PropertyType;
//
//			return (prop != null);
		}
	}

}
