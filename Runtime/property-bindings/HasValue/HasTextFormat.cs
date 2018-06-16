using BeatThat.GetComponentsExt;
using BeatThat.Controllers;
using UnityEngine;
using System;


namespace BeatThat.Properties{
	/// <summary>
	/// Drives a text object by applying some format to input text. 
	/// 
	/// This implementation has the undesirable property that the controller that interacts with it needs
	/// to understand that some set of text properties belong to a format.
	/// For this reason, it is often better to use FormatDrivesText instead
	/// </summary>
	public class HasTextFormat : HasText, IDrive<IHasText>
	{
		#region implemented abstract members of HasValue

		public override bool sendsValueObjChanged { get { return false; } }

		#endregion

		public string m_text;

		[Multiline]
		public string m_format = "{0}";

		[Tooltip("Enable if you want to be able to truncate args in the output using {0:$MAX_LEN}, e.g. {0:6} (arg zero with max length of 6)")]
		public bool m_enableStringLimiter;

		[Tooltip("When string limiter is enabled, add ellipses if a string is truncated?")]
		public bool m_onStringLimitAddEllipsis = true;

		#region IDrive implementation
		public HasText m_driven;
		public IHasText driven { get { return m_driven?? (m_driven = this.GetSiblingComponent<HasText>()); } } 

		public object GetDrivenObject() { return this.driven; }

		public bool ClearDriven() { m_driven = null; return true; }
		#endregion

		#region implemented abstract members of HasText

		override public string value 
		{
			get { return m_text; }
			set {
				m_text = value;
				Format(m_text);
			}
		}


		#endregion
		public string format { get { return m_format; } set { m_format = value; } }

		public void Format(object s1)
		{
			this.driven.value = (m_enableStringLimiter)?
				string.Format(this.limiter, this.format, s1): string.Format(this.format, s1);


//			Debug.Log("[" + Time.frameCount + "] set text to " + this.driven.text);
		}

		private StringLimiter limiter { get { return m_onStringLimitAddEllipsis? StringLimiter.SHARED_ELLIPSIS: StringLimiter.SHARED; } }

		public void Format(object s1, object s2)
		{
			this.driven.value = (m_enableStringLimiter)?
				string.Format(this.limiter, this.format, s1, s2): string.Format(this.format, s1, s2);
			
//			Debug.Log("[" + Time.frameCount + "] set text to " + this.driven.text);
		}

		public void Format(object s1, object s2, object s3)
		{
			this.driven.value = (m_enableStringLimiter)?
				string.Format(this.limiter, this.format, s1, s2, s3): string.Format(this.format, s1, s2, s3);
			
//			Debug.Log("[" + Time.frameCount + "] set text to " + this.driven.text);
		}

		public void Format(params object[] args)
		{
			this.driven.value = (m_enableStringLimiter)?
				string.Format(this.limiter, this.format, args): string.Format(this.format, args);
			
//			Debug.Log("[" + Time.frameCount + "] set text to " + this.driven.text);
		}
			
		/// <summary>
		/// Frequently want to truncate variables in a format
		/// </summary>
		class StringLimiter : IFormatProvider, ICustomFormatter
		{
			public static readonly StringLimiter SHARED = new StringLimiter();
			public static readonly StringLimiter SHARED_ELLIPSIS = new StringLimiter(true);

			public StringLimiter(bool ellipsis = false)
			{
				this.ellipsis = ellipsis;
			}

			public bool ellipsis { get; set; }

			public object GetFormat(Type formatType)
			{
				return this;
			}

			public string Format(string format, object arg, IFormatProvider formatProvider)
			{
				string s = arg as string;
				if (s != null) {
					int length;
					if (int.TryParse(format, out length)) {
						if(s.Length <= length) {
							return s;
						}

						if(length >= 3 && this.ellipsis) {
							return string.Format("{0}...", s.Substring(0, length - 3).Trim());
						}
							
						return s.Substring (0, length).Trim();
					}
				}
				return string.Format("{0:" + format + "}", arg);
			}
		}
	}
}


