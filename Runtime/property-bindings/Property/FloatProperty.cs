using BeatThat.TransformPathExt;
using UnityEngine;


namespace BeatThat.Properties
{
    public class FloatProperty : FloatProp
	{
		public float m_value; // TODO: this shouldn't be public but good to see in Inspector. Move to editor class.

		[Tooltip("set FALSE if you want a param to hold its value across disable/enable")]
		public bool m_resetValueOnDisable = true;

		override protected float GetValue() { return m_value; }

        override protected void EnsureValue(float v) 
		{
			m_value = v;
		}

        override protected void _SetValue(float v) 
        { 
            m_value = v;
        }

		virtual protected void OnDidApplyAnimationProperties()
		{
#if DEBUG_UNSTRIP || UNITY_EDITOR
			if(m_debug) {
				Debug.Log("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::OnDidApplyAnimationProperties");
			}
#endif

			SetValue(m_value);
		}


		virtual protected void OnDisable()
		{
			if(m_resetValueOnDisable) {
#if UNITY_EDITOR || DEBUG_UNSTRIP
                if (m_debug)
                {
                    Debug.Log("[" + Time.frameCount + "][" + this.Path() + "] " 
                              + GetType() + "::OnDisable will reset value from " 
                              + m_value + " to " + m_resetValue);
                }
#endif
                m_value = m_resetValue;
			}
            else {
#if UNITY_EDITOR || DEBUG_UNSTRIP
                if (m_debug)
                {
                    Debug.Log("[" + Time.frameCount + "][" + this.Path() + "] "
                              + GetType() + "::OnDisable will NOT reset value property, current " + m_value);
                }
#endif
            }
		}

		override protected void Start()
		{
			base.Start ();
			this.didStart = true;
            SetValue(m_value);
		}

		private bool didStart { get; set; }

		override protected void OnEnable()
		{
            base.OnEnable();

			if(!this.didStart) {
				return;
			}

			SetValue(m_value);

			if(m_debug) {
				Debug.Log("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::OnEnable");
			}
		}


	}
}

