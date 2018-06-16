using BeatThat.TransformPathExt;
using BeatThat.Pools;
using System;
using BeatThat.Bindings;
using UnityEngine.Events;

namespace BeatThat.Properties
{
    abstract public class DrivenBoolProp<DriverType> : BoolProp where DriverType : IHasValueObjChanged
	{
		public DriverType m_driver;
		public bool m_value;

		private UnityEventBinding m_binding;

		public DriverType driver { get { return m_driver; } }

		override protected void Start()
		{
			base.Start ();
			this.didStart = true;
			Bind();
		}

		private bool didStart { get; set; }

		public bool isBound { get { return m_binding != null; } }

		private void Bind()
		{
			if(this.isBound) {
				return;
			}
			m_binding = StaticObjectPool<UnityEventBinding>.Get();
			m_binding.Bind(m_driver.onValueObjChanged, this.driverValueChangedAction);
		}

		private void Unbind()
		{
			if(m_binding != null) {
				m_binding.Unbind();
				StaticObjectPool<UnityEventBinding>.Return(m_binding);
				m_binding = null;
			}
		}

		void OnDisable()
		{
			Unbind();
		}

		void OnEnable()
		{
			if(this.didStart) {
				Bind();
			}
		}

		protected override void _SetValue (bool s)
		{
			throw new NotImplementedException ();
		}

		private void OnDriverValueChanged()
		{
			#if UNITY_EDITOR || BT_DEBUG_UNSTRIP
			if(m_debug) {
				UnityEngine.Debug.Log("[" + UnityEngine.Time.frameCount + "][" + this.Path() + "] " + GetType()
					+ "::OnDriverValue changed (driver=" + this.driver.GetType() + " " + (this.driver as UnityEngine.Component).Path() + ")");
			}
			#endif
			var myVal = this.value;
			if(m_value == myVal) {
				return;
			}
			m_value = myVal;
			SendValueChanged(m_value);
		}
		private UnityAction driverValueChangedAction { get { return m_driverValueChangedAction?? (m_driverValueChangedAction = this.OnDriverValueChanged); } }
		private UnityAction m_driverValueChangedAction;

		
	}
}


