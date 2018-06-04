using UnityEngine;
using BeatThat.PropertyExt;

namespace BeatThat
{
	/// <summary>
	/// A component that proxies another HasFloat
	/// Gets a special base class to make sure that when you drag 'driven' in the inspector
	/// that it doesn't end of driving itself
	/// </summary>
	public abstract class ProxiesFloat : DisplaysFloat, IDrive<HasFloat>
	{
		override protected void OnEnable()
		{
			base.OnEnable();
            if(m_driveProperty == null) {
                m_driveProperty = this.FindNonCircularTarget();
			}
		}

        public HasFloat driven { get { return m_driveProperty; } }

		public object GetDrivenObject() { return this.driven; }

        public bool ClearDriven() { m_driveProperty = null; return true; }

		#if UNITY_EDITOR
		virtual protected void Reset()
		{
            m_driveProperty = this.FindNonCircularTarget();
		}
		#endif
	}
}
