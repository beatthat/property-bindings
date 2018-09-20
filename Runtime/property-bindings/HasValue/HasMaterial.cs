using UnityEngine;

namespace BeatThat.Properties
{
    public abstract class HasMaterial : HasValue<Material>, IHasMaterial
	{
		public Material material { get { return this.value; } set { this.value = value; } }

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

