using BeatThat.TransformPathExt;
using UnityEngine;
using UnityEngine.UI;
using BeatThat;

namespace BeatThat.Properties{
	public abstract class HasMaterial : MonoBehaviour, IHasMaterial
	{
		public Material material { get { return this.value; } set { this.value = value; } }
		public abstract Material value { get; set; }

		public object valueObj { get { return this.value; } }

		public static HasMaterial FindOrAdd(GameObject go)
		{
			var hm = go.GetComponent<HasMaterial>();
			if(hm != null) {
				return hm;
			}

			// TODO: if we wanted to make this plug and play, could make HasMaterial impls discoverable/addable with attributes
			// if(go.GetComponent<Renderer>() != null) {
			// 	return go.AddComponent<RendererMaterial>();
			// }
			//
			// if(go.GetComponent<Graphic>() != null) {
			// 	return go.AddComponent<GraphicMaterial>();
			// }

			Debug.LogWarning("No Material provider (Renderer or Graphic) on GameObject " + go.Path());
			return null;
		}
	}
}

