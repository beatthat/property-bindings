using UnityEngine;

namespace BeatThat.Properties
{
	/// <summary>
	/// MonoBehaviour base class for IProperties interface.
	/// Has no implementation, but there needs to be a MonoBehaviour base class 
	/// to make IProperties assignable in Unity editor
	/// </summary>
	abstract public class PropertiesBase : MonoBehaviour, IProperties
	{
		abstract public bool Set<T> (string name, T value);
	}
}
