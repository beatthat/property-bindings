using UnityEngine;
using UnityEngine.Events;
using System;
using BeatThat.OptionalComponents;

namespace BeatThat
{
	public abstract class HasValue : MonoBehaviour, IHasValue, IHasValueObjChanged
	{
		[Tooltip("set TRUE to disable the behaviour that checks/ensures sibling components defined by the [OptionalComponent] attribute.")]
		public bool m_disableEnsureOptionalComponentsOnStart;

		virtual protected void Start()
		{
			HandleOptionalComponents ();
		}

		virtual protected void HandleOptionalComponents()
		{
			if (!m_disableEnsureOptionalComponentsOnStart) {
				this.EnsureAllOptionalComponents ();
			}
		}

		/// <summary>
		/// TRUE if the impl class actually sends the onValueObjChanged event.
		/// </summary>
		abstract public bool sendsValueObjChanged { get; }


		/// <summary>
		/// A general-purpose event for value changed. 
		/// Most subclasses will have a specific value-changed event.
		/// The main case for the existence of this event is when you a component to have a Unity-Editor-configurable array
		/// of values, e.g. a FormatDrivesText wants an array of values and needs to know when a value changes.
		/// </summary>
		public UnityEvent onValueObjChanged { get { return m_onValueObjChanged?? (m_onValueObjChanged = new UnityEvent()); } }

		/// <summary>
		/// CONTRACT: subclasses that return sendsValueObjChanged TRUE must call this when their value changes.
		/// </summary>
		protected void SendValueObjChanged()
		{
			if(m_onValueObjChanged != null) {
				m_onValueObjChanged.Invoke();
			}
		}
		[NonSerialized]private UnityEvent m_onValueObjChanged;

		abstract public object valueObj { get; }
	}
}
