using UnityEngine;
using UnityEngine.Events;

namespace BeatThat
{
	/// <summary>
	/// Make a local property the driven selector of a property of the same value type
	/// 
	/// By default, looks for the driver property in the parent chain of GameObjects,
	/// but may be overidden to find drivers in any way. including swapping drivers as events occur. 
	/// </summary>
	[RequireComponent(typeof(SourceProp))]
	public class BindPropToProp<SourceProp, TgtProp, ValueType> : PropertyBinding<TgtProp>
		where SourceProp : class, IHasValue<ValueType>, IHasValueChangedEvent<ValueType>
		where TgtProp : class, IHasValue<ValueType>
	{
		[Tooltip("link the driver manually to a scene (or coprefab) object. This should generally override the FindDriver process")]
		public SourceProp m_driver;

		[Tooltip("the value set when an update occurs and the driver is null")]
		public ValueType m_defaultValue;

		[Tooltip("if set TRUE, then will not auto update when driver value changes, and instead only when UpdateProperty is called (useful for animated updates).")]
		public bool m_disableAutoSync;

		[Tooltip("set TRUE if it's valid to have a null driver, e.g. the driver is a prop in the selected item of some list")]
		public bool m_logWarningOnNoDriver = true;

		public bool m_debug;

		virtual protected ValueType defaultValue { get { return m_defaultValue; } }
		virtual protected void SetDefaultValue(ValueType v)
		{
			m_defaultValue = v;
		}

		sealed override protected void BindProperty ()
		{
			BindP2P();
			RebindDriver();
		}

		sealed override protected void UnbindProperty()
		{
			UnbindDriver();
			UnbindP2P();
		}

		/// <summary>
		/// Convenience method to take a target property and connect it to a driver 
		/// by adding and configuring a BindPropToProp component
		/// to the target property's GameObject.
		/// </summary>
		public static bool Connect<BindP2PType>(SourceProp driver, TgtProp tgtProp, ValueType resetValue = default(ValueType))
			where BindP2PType : BindPropToProp<SourceProp, TgtProp, ValueType>
		{
			if(tgtProp == null) {
				#if UNITY_EDITOR || DEBUG_UNSTRIP
				Debug.LogWarning("Unable to Connect to a null target prop!");
				#endif
				return false;
			}
				
			var tgtPropComp = tgtProp as Component;
			if(tgtPropComp == null) {
				#if UNITY_EDITOR || DEBUG_UNSTRIP
				Debug.LogWarning("Unable to Connect to a target prop that's not a component!");
				#endif
				return false;
			}

			var binding = tgtPropComp.gameObject.AddComponent<BindP2PType> ();
			binding.SetDefaultValue(resetValue);
			binding.ConfigureDriver (driver);
			binding.property = tgtProp;

			return true;
		}

		/// <summary>
		/// override to add behaviour on bind
		/// </summary>
		virtual protected void BindP2P() {}

		/// <summary>
		/// override to add behaviour on unbind
		/// </summary>
		virtual protected void UnbindP2P() {}


		public void UpdateProperty()
		{
			var d = this.driver;
			var p = this.property;
			var v = (d != null)? d.value: this.defaultValue;

			#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
			if(m_debug) {
				Debug.Log("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() 
					+ "::UpdateProperty driver=" + d + ", property=" + p + ", newVal=" + v);
			}
			#endif

			#if UNITY_EDITOR
			if(p == null && !Application.isPlaying) {
				// update from inspector, we need to find the prop
				PropertyBindingHelper.GetProperty<TgtProp>(this, out p);
			}
			#endif

			if(p == null) {
				#if UNITY_EDITOR || BT_DEBUG_UNSTRIP
				Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() 
					+ " unable to update (target prop is null)");
				#endif
				return;
			}

			p.value = v;
		}

		/// <summary>
		/// Called on every RebindDriver to find the driver.
		/// Default implementation searches parents.
		/// Override to find the driver component property some other way.
		/// NOTE: would be nice to return generic DriverType here, but often you get crash problems mixing generics and virtual functions
		/// </summary>
		/// <returns>The driver property.</returns>
		virtual protected object FindDriverProperty()
		{
			return m_driver ?? this.transform.parent.GetComponentInParent<SourceProp> (true);
		}

		/// <summary>
		/// Call this method when some event has occurred that indicates the driver may have changed.
		/// Unbinds the old driver (if any bound), finds new driver, binds it and updates the value.
		/// </summary>
		public void RebindDriver()
		{
			UnbindDriver();

			var d = FindDriverProperty() as SourceProp;
			if(d == null) {
				if(m_logWarningOnNoDriver) {
					Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "] unable to find driver instance of " + typeof(SourceProp)
						+ ". If it's valid to have a null driver, set logWarningOnNoDriver to FALSE");
				}
				UpdateProperty();
				return;
			}

			this.driver = d;

			var b = StaticObjectPool<UnityEventBinding<ValueType>>.Get();
			b.Bind(d.onValueChanged, this.driverValueChangedAction);

			this.driverBinding = b;

			UpdateProperty();
		}

		private void UnbindDriver()
		{
			var b = this.driverBinding;
			this.driverBinding = null;
			if(b != null) {
				b.Unbind();
				StaticObjectPool<UnityEventBinding<ValueType>>.Return(b);
			}
			this.driver = null;
		}

		public void ConfigureDriver(SourceProp d)
		{
			m_driver = d;
			this.driver = d;
		}

		public SourceProp driver { get { return m_driverRef.value; } set { m_driverRef = new SafeRef<SourceProp>(value); } }
		private SafeRef<SourceProp> m_driverRef;
		private UnityEventBinding<ValueType> driverBinding { get; set; }


		private void OnDriverValueChanged(ValueType value)
		{
			#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
			if(m_debug) {
				Debug.Log("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::OnDriverValueChanged");
			}
			#endif

			if(!m_disableAutoSync) {
				UpdateProperty();
			}
		}
		private UnityAction<ValueType> driverValueChangedAction { get { return m_driverValueChangedAction?? (m_driverValueChangedAction = this.OnDriverValueChanged); } }
		private UnityAction<ValueType> m_driverValueChangedAction;
	}

	[RequireComponent(typeof(PropType))]
	public class BindPropToProp<PropType, ValueType> : BindPropToProp<PropType, PropType, ValueType>
		where PropType : class, IHasValue<ValueType>, IHasValueChangedEvent<ValueType> 
	{
	}

}