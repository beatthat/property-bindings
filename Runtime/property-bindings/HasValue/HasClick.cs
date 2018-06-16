using UnityEngine;
using UnityEngine.Events;

namespace BeatThat.Properties{
	public abstract class HasClick : MonoBehaviour, IHasClick
	{
		#if LEGACY_HAS_CLICK
		public abstract bool interactable { get; set; }

		[System.Obsolete("use onClick UnityEvent")]
		public event System.Action Clicked;
		#endif

		[SerializeField] private UnityEvent m_onClicked = new UnityEvent();
		public UnityEvent onClicked { get { return m_onClicked; } set { m_onClicked = value; } }

		protected void SendClickEvent()
		{
			#if LEGACY_HAS_CLICK
			#pragma warning disable 618
			if(this.Clicked != null) {
				Clicked();
			}
			#pragma warning restore 618
			#endif

			this.onClicked.Invoke();
		}
	}
}
