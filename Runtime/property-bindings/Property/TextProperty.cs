using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine;


namespace BeatThat
{
	public class TextProperty : TextProp
	{
		[FormerlySerializedAs("m_text")]
		public string m_value;

		override protected string GetValue() { return m_value; }
		override protected void _SetValue(string s) { m_value = s; }
		
	}

	public abstract class TextProp : HasText, IHasValueChangedEvent<string>
	{
		public bool m_debug;
		public bool m_debugBreakOnSetValue;

		public UnityEvent<string> onValueChanged 
		{ 
			get { return m_onValueChanged?? (m_onValueChanged = new StringEvent()); } 
			set { m_onValueChanged = value; } 
		}
		[SerializeField]protected UnityEvent<string> m_onValueChanged;

		override public string value
		{ 
			get { return GetValue(); }
			set { SetValue(value); } 
		}

		override public object valueObj { get { return this.value; } }

		abstract protected string GetValue();
		abstract protected void _SetValue(string s);

		override public bool sendsValueObjChanged { get { return true; } }

		protected void SendValueChanged(string val)
		{
			SendValueObjChanged();
			if(m_onValueChanged != null) {
				m_onValueChanged.Invoke(val);
			}
		}

		protected void SetValue(string val, PropertyEventOptions opts = PropertyEventOptions.SendOnChange)
		{
			#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
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
				SendValueChanged(val);
			}
		}
	}
}
