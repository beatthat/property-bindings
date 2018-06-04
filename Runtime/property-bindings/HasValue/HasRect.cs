using UnityEngine;

namespace BeatThat
{
	public abstract class HasRect : MonoBehaviour, IHasRect
	{
		public abstract Rect rect { get; set; }
	}
}
