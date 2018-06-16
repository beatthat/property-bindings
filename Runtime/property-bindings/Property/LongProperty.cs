using BeatThat.TransformPathExt;
using UnityEngine.Events;
using UnityEngine;
using BeatThat.UnityEvents;

namespace BeatThat.Properties{
    public interface ILongProp : IHasProp<long> {}

	public class LongProperty : LongProp
	{
		public long m_value; // TODO: this shouldn't be public but good to see in Inspector. Move to editor class.

		override protected long GetValue() { return m_value; }
		override protected void _SetValue(long s) { m_value = s; }
	}

    public abstract class LongProp : HasLong, ILongProp
	{
		public bool m_debug;
		public bool m_debugBreakOnSetValue;

		public UnityEvent<long> onValueChanged 
		{ 
			get { return m_onValueChanged?? (m_onValueChanged = new LongEvent()); } 
			set { m_onValueChanged = value; } 
		}
		[SerializeField]protected UnityEvent<long> m_onValueChanged;

		override public long value
		{ 
			get { return GetValue(); }
			set { SetValue(value); } 
		}
			
		override public object valueObj { get { return this.value; } }

		abstract protected long GetValue();
		abstract protected void _SetValue(long s);

		override public bool sendsValueObjChanged { get { return true; } }

		protected void SetValue(long val, PropertyEventOptions opts = PropertyEventOptions.SendOnChange)
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

