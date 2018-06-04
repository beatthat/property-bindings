using UnityEngine;
using UnityEngine.Events;
using System;

namespace BeatThat
{
	public abstract class HasTexture : MonoBehaviour, IEditsTexture
	{
		public UnityEvent onValueChanged { get { return m_onValueChanged?? (m_onValueChanged = new UnityEvent()); } }
		private UnityEvent m_onValueChanged; 


		[ObsoleteAttribute("use value property instead")]
		public Texture texture 
		{
			get { return GetTexture(); } 
			set {
				if(GetTexture() == value) {
					return;
				}
				SetTexture(value);
				if(m_onValueChanged != null) {
					m_onValueChanged.Invoke();
				}
			}
		}

		public Texture value
		{
			get { return GetTexture(); } 
			set {
				if(GetTexture() == value) {
					return;
				}
				SetTexture(value);
				if(m_onValueChanged != null) {
					m_onValueChanged.Invoke();
				}
			}
		}

		public object valueObj { get { return this.value; } }
		abstract protected Texture GetTexture();
		abstract protected void SetTexture(Texture t);
	}
}
