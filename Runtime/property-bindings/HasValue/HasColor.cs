using UnityEngine;

namespace BeatThat.Properties{
	public abstract class HasColor : HasValue, IHasColor
	{
		public abstract Color value { get; set; }
	}
}
