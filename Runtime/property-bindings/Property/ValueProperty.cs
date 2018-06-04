using UnityEngine.Events;
using UnityEngine;
using System;
using System.Collections.Generic;


namespace BeatThat
{
	public class ValueProperty<T> : ValueProp<T>
	{
		public T m_value; // TODO: this shouldn't be public but good to see in Inspector. Move to editor class.

		override protected T GetValue() { return m_value; }
		override protected void _SetValue(T s) { m_value = s; }
	}

	/// <summary>
	/// Base class for any custom value property that doesn't need an editor-assignable HasXXX base class.
	/// </summary>
	abstract public class ValueProp<T> : HasValue, IHasValue<T>, IHasValueChangedEvent<T>
	{
		public bool m_debug;
		public bool m_debugBreakOnSetValue;

		public UnityEvent<T> onValueChanged 
		{ 
			get { return m_onValueChanged?? (m_onValueChanged = new MyEvent()); } 
			set { m_onValueChanged = value; } 
		}
		[SerializeField]protected UnityEvent<T> m_onValueChanged;
		[Serializable]class MyEvent : UnityEvent<T> {}

		public T value
		{ 
			get { return GetValue(); }
			set { SetValue(value); } 
		}

		override public object valueObj { get { return this.value; } }

		abstract protected T GetValue();
		abstract protected void _SetValue(T s);

		override public bool sendsValueObjChanged { get { return true; } }

		protected void SetValue(T val, PropertyEventOptions opts = PropertyEventOptions.SendOnChange)
		{
			#if BT_DEBUG_UNSTRIP
			if(m_debug) {
			Debug.Log("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::set_value to " + val);
			}
			#endif

			if(EqualityComparer<T>.Default.Equals(val, GetValue()) && opts != PropertyEventOptions.Force) {
				return;
			}

			_SetValue(val);

			#if UNITY_EDITOR
			if(m_debugBreakOnSetValue) {
				Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::set_value to " + val + " BREAK ON SET VALUE is enabled");
				Debug.Break();
			}
			#endif

			if(opts != PropertyEventOptions.Disable) {
				SendValueObjChanged();
				if(m_onValueChanged != null) {
					m_onValueChanged.Invoke(val);
				}
			}
		}
	}

}
