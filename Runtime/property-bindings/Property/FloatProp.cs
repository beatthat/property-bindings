using BeatThat.TransformPathExt;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.Serialization;
using BeatThat.UnityEvents;

namespace BeatThat.Properties{

    public interface IFloatProp : IHasProp<float> {}

    public abstract class FloatProp : HasFloat, IFloatProp
	{
		public bool m_debug;
		public bool m_debugBreakOnSetValue;

		[FormerlySerializedAs("m_value")]
		[Tooltip("the prop will resume this value OnDisable (when resetDefaultValueOnDisable is set)")]
		public float m_resetValue;


		[Tooltip("set as 'BindToProperty' to use another property to drive the value of this property; set as 'DriveProperty' to use this property to drive the value of another property")]
		[HideInInspector][FormerlySerializedAs("m_enablePropertyBinding")]public BindOrDrivePropertyOptions m_bindOrDrivePropertyOptions;
		public BindOrDrivePropertyOptions bindOrDrivePropertyOptions { get { return m_bindOrDrivePropertyOptions; } set { m_bindOrDrivePropertyOptions = value; } }
		[HideInInspector]public FloatProp m_bindToProperty;
        [HideInInspector][FormerlySerializedAs("m_driven")]public HasFloat m_driveProperty;
		private bool hasConnectedBinding;
		virtual protected void OnEnable()
		{
            // NOTE: this needs to be OnEnable (not awake or start)
            // because otherwise if this component exists on, say,
            // a pooled object that's getting enabled and disabled, 
            // the Connect logic below will ONLY execute on first use of object.

            if (this.bindOrDrivePropertyOptions == BindOrDrivePropertyOptions.Disabled)
            {
#if UNITY_EDITOR || DEBUG_UNSTRIP
                if (m_debug)
                {
                    Debug.Log("[" + this.Path() + "] " + GetType() + " bind or drive property is disabled");
                }
#endif
                return;
            }

            if (this.hasConnectedBinding && this.bindOrDrivePropertyOptions == BindOrDrivePropertyOptions.BindToProperty && m_bindToProperty != null)
            {
#if UNITY_EDITOR || DEBUG_UNSTRIP
                if (m_debug)
                {
                    Debug.Log("[" + this.Path() + "] " + GetType() + " bind or drive property is already connected. Setting value to " + m_bindToProperty.value);
                }
#endif
                SetValue(m_bindToProperty.value);
                return;
            }

			this.hasConnectedBinding |= 
				this.bindOrDrivePropertyOptions == BindOrDrivePropertyOptions.BindToProperty 
				&& m_bindToProperty != null 
				&& BindFloatToFloat.Connect<BindFloatToFloat> (m_bindToProperty, this, m_resetValue);

			this.hasConnectedBinding |= 
				this.bindOrDrivePropertyOptions == BindOrDrivePropertyOptions.DriveProperty
				&& m_driveProperty != null
				&& BindFloatToFloat.Connect<BindFloatToFloat> (this, m_driveProperty, m_resetValue);


#if UNITY_EDITOR || DEBUG_UNSTRIP
            if (this.hasConnectedBinding)
            {
                if (m_debug)
                {
                    Debug.Log("[" + Time.frameCount + "] connect succeeded on enabsle " + GetType() + "[" + this.Path() + "]");
                }
            }
            else {
				Debug.LogWarning("[" + Time.frameCount + "] failed to bind to prop at " + GetType() + "[" + this.Path() + "]");
			}
#endif
		}

		public UnityEvent<float> onValueChanged 
		{ 
			get { return m_onValueChanged?? (m_onValueChanged = new FloatEvent()); } 
			set { m_onValueChanged = value; } 
		}
		[SerializeField]protected UnityEvent<float> m_onValueChanged;

		override public float value
		{ 
			get { return GetValue(); }
			set { SetValue(value); } 
		}
			

		override public object valueObj { get { return this.value; } }
		// <summary>
		/// Override for case where you might want to store a value even if the value currently returned matches the new value set.
		/// This is the case for working with Animator params.
		/// </summary>
        virtual protected void EnsureValue (float v) {}
		abstract protected float GetValue();
        abstract protected void _SetValue(float v);

		override public bool sendsValueObjChanged { get { return true; } }

        protected void SendValueChanged(float v)
		{
			SendValueObjChanged();
			if(m_onValueChanged != null) {
				m_onValueChanged.Invoke(v);
			}
		}

        public void SetValue(float v, PropertyEventOptions opts = PropertyEventOptions.SendOnChange)
		{
#if DEBUG_UNSTRIP || UNITY_EDITOR
			if(m_debug) {
				Debug.Log("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::set_value to " + v);
			}
#endif

            if(opts != PropertyEventOptions.Force && Mathf.Approximately(v, GetValue())) {
				EnsureValue (v);

#if DEBUG_UNSTRIP || UNITY_EDITOR
                if (m_debug)
                {
                    Debug.Log("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::set_value to " + v + " will skip update (value not changed)");
                }
#endif
				return;
			}

			_SetValue(v);

			#if UNITY_EDITOR
			if(m_debugBreakOnSetValue) {
				Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::set_value to " + v + " BREAK ON SET VALUE is enabled");
				Debug.Break();
			}
			#endif

			if(opts != PropertyEventOptions.Disable) {
				SendValueChanged(v);
			}
		}


        virtual protected void OnDestroy()
        {
#if DEBUG_UNSTRIP || UNITY_EDITOR
            if (m_debug)
            {
                Debug.Log("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::OnDestroy");
            }
#endif
        }
	}

}

