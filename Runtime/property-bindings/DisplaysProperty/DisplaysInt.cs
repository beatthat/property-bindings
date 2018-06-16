using UnityEngine;

namespace BeatThat.Properties{
	/// <summary>
	/// Use an int to drive some display element and an update-display call will trigger when the property changes by script call or animation.
	/// 
	/// Exposes the IHasFloat set_value interface, so this component can be used more easily in transitions (e.g. as an element of a TransitionsElements)
	/// </summary>
	public abstract class DisplaysInt : HasInt
	{
		public int m_value;
		public bool m_updateDisplayOnEnable = true;

		public override int value 
		{
			get {
				return m_value;
			}
			set {
				var changed = m_value != value;
				m_value = value;
				UpdateDisplay();
				if(changed) {
					SendValueObjChanged();
				}
			}
		}

		override public object valueObj { get { return this.value; } }

		void OnEnable()
		{
			if(m_updateDisplayOnEnable) {
				UpdateDisplay();
			}
		}

		override public bool sendsValueObjChanged { get { return true; } }

		abstract public void UpdateDisplay();

		void OnDidApplyAnimationProperties()
		{
			if(!this.gameObject.activeInHierarchy) {
				return;
			}
			UpdateDisplay();
		}
	}
}
