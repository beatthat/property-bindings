#if LEGACY_EDITS_BOOL
using UnityEngine.Events;


namespace BeatThat.Properties{
	public abstract class EditsBool : HasBool, IEditsBool
	{
		public UnityEvent onValueChanged { get { return m_onValueChanged?? (m_onValueChanged = new UnityEvent()); } set { m_onValueChanged = value; } }
		private UnityEvent m_onValueChanged;

		//public abstract bool interactable { get; set; }

		protected void SendValueChanged()
		{
			this.onValueChanged.Invoke();
		}
	}
}
#endif