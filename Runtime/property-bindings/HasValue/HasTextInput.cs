using UnityEngine.Events;


namespace BeatThat
{
	public abstract class HasTextInput : TextProp, IHasTextInput
	{
		public UnityEvent<string> onSubmit { get { return m_onSubmit; } }
		public StringEvent m_onSubmit = new StringEvent();

		public abstract void ActivateInput();
	}
}
