using BeatThat.TransformPathExt;
using BeatThat.Controllers;
using UnityEngine;
using UnityEngine.Events;

namespace BeatThat.Properties{
	/// <summary>
	/// Base class for components that listens for changes in a value property
	/// </summary>
	abstract public class ValueObserver<InputType, ValueType> : Subcontroller 
		where InputType : HasValue, IHasValue<ValueType>, IHasValueChangedEvent<ValueType>
	{
		public InputType m_input;

		[Tooltip("perform the update op on bind")]
		public bool m_updateOnBind = true;

		public bool m_debug;
			
		public InputType input { get { return m_input; } }

		sealed override protected void BindSubcontroller()
		{
			if(m_updateOnBind) {
				UpdateWithInputValue(this.input.value);
			}
			Bind(m_input.onValueChanged, this.inputValueChangedAction);

			BindObserver();
		}

		/// <summary>
		/// Override BindObserver to add additional bind logic for a subclass
		/// </summary>
		virtual protected void BindObserver() {}

		sealed override protected void UnbindSubcontroller()
		{
			UnbindObserver();
		}

		/// <summary>
		/// Override UnbindObserver to add additional unbind logic for a subclass
		/// </summary>
		virtual protected void UnbindObserver() {}


		abstract protected void UpdateWithInputValue(ValueType v);

		private void OnInputValueChanged(ValueType v)
		{
			#if BT_DEBUG_UNSTRIP
			if(m_debug) {
				Debug.Log("[" + Time.frameCount + "][" + this.Path() + "] input value changed");
			}
			#endif
			UpdateWithInputValue(v);
		}
		private UnityAction<ValueType> inputValueChangedAction { get { return m_inputValueChangedAction?? (m_inputValueChangedAction = this.OnInputValueChanged); } }
		private UnityAction<ValueType> m_inputValueChangedAction;


	}
}


