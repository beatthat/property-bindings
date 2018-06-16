using UnityEngine;
using System;

namespace BeatThat.Properties{
	public abstract class HasDateTime : HasValue, IHasDateTime
	{
		public abstract DateTime value { get; set; }

		override public object valueObj { get { return this.value; } }
	}
}
