using System;
using UnityEngine.Events;


namespace BeatThat.Properties{
	public abstract class EditsFloat : HasFloat, IHasValueChangedEvent
	{
		public UnityEvent onValueChanged { get { return m_onValueChanged; } }
		public UnityEvent m_onValueChanged = new UnityEvent();

//		[Obsolete("use UnityEvent onValueChanged")]public event Action ValueChanged;

		public abstract bool interactable { get; set; }

		protected void SendValueChanged()
		{
			this.onValueChanged.Invoke();
//			Actions.Exec(this.ValueChanged);
		}
	}
}
