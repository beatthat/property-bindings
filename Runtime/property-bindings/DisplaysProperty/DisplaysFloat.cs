using UnityEngine;

namespace BeatThat.Properties{
	/// <summary>
	/// Use a float to drive some display element and an update-display call will trigger when the property changes by script call or animation.
	/// Exposes the IHasFloat set_value interface, so this component can be used more easily in transitions (e.g. as an element of a TransitionsElements)
	/// </summary>
	public abstract class DisplaysFloat : FloatProperty
	{
		public bool m_updateDisplayOnEnable;
		public bool m_applyChangesOnLateUpdate;

		override protected void EnsureValue(float val) 
		{
			base.EnsureValue (val);
			ScheduleUpdateDisplay ();
		}

		override protected void _SetValue(float val) 
		{
			EnsureValue (val);
		}
			
		// Analysis disable ConvertToAutoProperty
		virtual protected bool applyChangesOnLateUpdate { get { return m_applyChangesOnLateUpdate; } set { m_applyChangesOnLateUpdate = value; } }
		// Analysis restore ConvertToAutoProperty

		override protected void OnEnable()
		{
			base.OnEnable ();
			if(m_updateDisplayOnEnable) {
				UpdateDisplay();
			}
		}

		protected void ScheduleUpdateDisplay()
		{
			if(this.applyChangesOnLateUpdate) {
				this.displayUpdatePending = true;
				return;
			}
			UpdateDisplay();
		}

		abstract public void UpdateDisplay();

		protected bool displayUpdatePending { get; set; }

		virtual protected void LateUpdate()
		{
			if(this.displayUpdatePending) {
				UpdateDisplay();
				this.displayUpdatePending = false;
			}
		}
	}
}
