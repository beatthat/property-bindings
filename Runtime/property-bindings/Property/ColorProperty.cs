using UnityEngine;
using UnityEngine.Events;

namespace BeatThat
{
//	public class ColorProperty : HasColor
//	{
//		public Color m_color;
//
//		#region implemented abstract members of HasColor
//		override protected Color GetColor() 
//		{
//			return m_color;
//		}
//
//		override protected void _SetColor(Color c)
//		{
//			m_color = c;
//		}
//		#endregion
//	}

	public class ColorProperty : ColorProp
	{
		public Color m_value; // TODO: this shouldn't be public but good to see in Inspector. Move to editor class.

		override protected Color GetValue() { return m_value; }
		override protected void _SetValue(Color s) { m_value = s; }
	}

	public abstract class ColorProp : HasColor, IHasValueChangedEvent<Color>
	{
		public bool m_debug;
		public bool m_debugBreakOnSetValue;

		public UnityEvent<Color> onValueChanged 
		{ 
			get { return m_onValueChanged?? (m_onValueChanged = new ColorEvent()); } 
			set { m_onValueChanged = value; } 
		}
		[SerializeField]protected UnityEvent<Color> m_onValueChanged;

		override public Color value
		{ 
			get { return GetValue(); }
			set { SetValue(value); } 
		}

		override public object valueObj { get { return this.value; } }

		abstract protected Color GetValue();
		abstract protected void _SetValue(Color s);

		override public bool sendsValueObjChanged { get { return true; } }

		protected void SetValue(Color val, PropertyEventOptions opts = PropertyEventOptions.SendOnChange)
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
