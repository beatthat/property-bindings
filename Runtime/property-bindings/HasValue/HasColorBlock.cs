using UnityEngine.UI;

namespace BeatThat.Properties{
	public abstract class HasColorBlock : HasValue, IHasColorBlock
	{
		public abstract ColorBlock value { get; set; }
	}
}
