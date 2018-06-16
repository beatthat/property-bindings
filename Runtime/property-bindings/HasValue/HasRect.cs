using UnityEngine;

namespace BeatThat.Properties{
	public abstract class HasRect : MonoBehaviour, IHasRect
	{
		public abstract Rect rect { get; set; }
	}
}
