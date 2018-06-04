using UnityEngine.UI;

namespace BeatThat
{
	public abstract class HasColorBlock : HasValue, IHasColorBlock
	{
		public abstract ColorBlock value { get; set; }
	}
}
