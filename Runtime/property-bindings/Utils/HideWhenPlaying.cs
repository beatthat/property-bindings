using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAL3
{
	/// <summary>
	/// Mainly for test scenes. 
	/// Add to a component that should display only when unity is NOT playing, 
	/// e.g. some README text that explains what the test scene is for.
	/// </summary>
	public class HideWhenPlaying : MonoBehaviour
	{

		void Awake()
		{
			this.gameObject.SetActive (false);
		}
	}
}