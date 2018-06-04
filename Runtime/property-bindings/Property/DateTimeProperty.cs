using UnityEngine.Events;
using UnityEngine;
using System;


namespace BeatThat
{
	public class DateTimeProperty : DateTimeProp
	{
		public DateTime m_value; // TODO: this shouldn't be public but good to see in Inspector. Move to editor class.

		override protected DateTime GetValue() { return m_value; }
		override protected void _SetValue(DateTime s) { m_value = s; }
	}

	public abstract class DateTimeProp : HasDateTime, IHasValueChangedEvent<DateTime>
	{
		public bool m_debug;
		public bool m_debugBreakOnSetValue;

		public UnityEvent<DateTime> onValueChanged 
		{ 
			get { return m_onValueChanged?? (m_onValueChanged = new DateTimeEvent()); } 
			set { m_onValueChanged = value; } 
		}
		[SerializeField]protected UnityEvent<DateTime> m_onValueChanged;

		override public DateTime value
		{ 
			get { return GetValue(); }
			set { SetValue(value); } 
		}

		abstract protected DateTime GetValue();
		abstract protected void _SetValue(DateTime s);

		override public bool sendsValueObjChanged { get { return true; } }

		protected void SetValue(DateTime val, PropertyEventOptions opts = PropertyEventOptions.SendOnChange)
		{
			#if BT_DEBUG_UNSTRIP
			if(m_debug) {
				Debug.Log("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::set_value to " + val);
			}
			#endif

			if(val == GetValue() && opts != PropertyEventOptions.Force) {
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
