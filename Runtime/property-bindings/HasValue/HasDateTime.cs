using UnityEngine;
using System;

namespace BeatThat
{
	public abstract class HasDateTime : HasValue, IHasDateTime
	{
		public abstract DateTime value { get; set; }

		override public object valueObj { get { return this.value; } }
	}
}
