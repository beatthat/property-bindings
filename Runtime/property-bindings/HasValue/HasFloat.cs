using UnityEngine;

namespace BeatThat.Properties{
	public abstract class HasFloat : HasValue, IHasFloat
	{
		public abstract float value { get; set; }

		override public object valueObj { get { return this.value; } }
	}
}
