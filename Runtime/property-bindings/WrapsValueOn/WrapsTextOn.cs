using UnityEngine;

namespace BeatThat
{
	// TODO: create DrivesFloatOn and DrivesIntOn as needed

	/// <summary>
	/// HasText wrapper base class for an arbitraty wrapped (DrivenType) component.
	/// 
	/// Using this base class saves some repetitive code, and, 
	/// because this base class implements both IHasValue<string> and IDrive<T>
	/// it will be discovered and added automatically in the following circumstance: 
	/// 
	/// 1. a PropertyBinding component is looking to target a string
	/// 2. the PropertyBinding component is a sibling component of type DrivenType
	/// 
	/// For a more concrete example if you have a GameObject that already has a TextMeshPro text component,
	/// and you add a FormatDrivesText component (which extends PropertyBinding<IHasText>)
	/// the tools will automatically discover and add wrapper type TMPHasText (because it implements IDrive<TextMeshPro>).
	/// 
	/// NOTE on this class being just for text:
	/// 
	/// It would be possible to have a single class for all value types, e.g.
	///  	<code>DrivesValueOn<DrivenType, ValueType> : HasValue</code>
	/// ...the reason it's done this way instead is to preserve the unity assignability of this class as a HasText.
	/// 
	/// Subclasses generally need to override only GetValue and HasValue.
	/// 
	/// </summary>
	public abstract class WrapsTextOn<WrappedType> : HasText, IDrive<WrappedType>
		where WrappedType : class
	{
		[Tooltip("The (target) wrapped object whose text value will be exposed by this HasText")]
		public WrappedType m_wrapped;

		public WrappedType driven { get { return ResolveWrapped(); } }

		/// <summary>
		/// Override to get the value from the driven object, which is guaranteed to be not null here.
		/// Passes the driven object as type <code>object</code> instead of <code>DrivenType</code>
		/// because historically, some target platforms can't handle overriden methods with generic types in the signature.
		/// </summary>
		/// <returns>The value.</returns>
		/// <param name="driven">The wrapped object which holds the value</param>
		abstract protected string GetValue (object driven);
		abstract protected void SetValue (object driven, string value);

		public override bool sendsValueObjChanged { get { return false; } }

		override public string value 
		{ 
			get { 
				var d = this.GetDrivenObject ();
				return d != null ? GetValue (driven) : null;
			} 
			set { 
				var d = this.GetDrivenObject ();
				if (d != null) {
					SetValue (d, value);
				}
			} 
		}

		public object GetDrivenObject ()
		{
			return this.driven;
		}

		public bool ClearDriven ()
		{
			m_wrapped = null;
			m_wrappedResolved.value = null;
			return true;
		}

		virtual protected bool FindWrapped(out object target)
		{
			return PropertyBindingHelper.FindOrAddTarget(this, typeof(WrappedType), out target);
		}

		virtual protected WrappedType ResolveWrapped()
		{
			if (m_wrappedResolved.isValid) {
				return m_wrappedResolved.value;
			}

			if (m_wrapped != null) {
				m_wrappedResolved = new SafeRef<WrappedType> (m_wrapped);
				return m_wrapped;
			}

			object d;
			if (!FindWrapped (out d)) {
				#if UNITY_EDITOR || DEBUG_UNSTRIP
				Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "] unable to resolve driven target with type " + typeof(WrappedType).Name);
				#endif
				return null;
			}
				
			var dres = d as WrappedType;
			m_wrappedResolved = new SafeRef<WrappedType> (dres);
			return dres;
		}

		private SafeRef<WrappedType> m_wrappedResolved;
	}
}
