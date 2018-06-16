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
		virtual protected void Awake()
		{
			if (this.bindOrDrivePropertyOptions == BindOrDrivePropertyOptions.Disabled || this.hasConnectedBinding) {
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
			if(!this.hasConnectedBinding) {
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
		virtual protected void EnsureValue (float s) {}
		abstract protected float GetValue();
		abstract protected void _SetValue(float s);

		override public bool sendsValueObjChanged { get { return true; } }

		protected void SendValueChanged(float val)
		{
			SendValueObjChanged();
			if(m_onValueChanged != null) {
				m_onValueChanged.Invoke(val);
			}
		}

		public void SetValue(float val, PropertyEventOptions opts = PropertyEventOptions.SendOnChange)
		{
			#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
			if(m_debug) {
				Debug.Log("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::set_value to " + val);
			}
			#endif

            if(Mathf.Approximately(val, m_valueLastSet) && opts != PropertyEventOptions.Force) {
				EnsureValue (val);
				return;
			}

			_SetValue(val);

            m_valueLastSet = val;

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


        private float m_valueLastSet; // need this to know if value has changed via animation
	}

}

