using UnityEngine;

namespace BeatThat
{
	public class BindColorToColor : BindPropToProp<ColorProp, IHasColor, HasColor, Color>, IHasColorAsset
	{
		[SerializeField][HideInInspector]private bool m_useAssetForDefaultValue;
		[SerializeField][HideInInspector]private ColorAsset m_defaultValueAsset;

		override public Color defaultValue 
		{
			get { 
				return (m_useAssetForDefaultValue && m_defaultValueAsset != null)? 
					m_defaultValueAsset.value: base.defaultValue; 
			} 
		}

		public ColorAsset colorAsset { get { return m_defaultValueAsset; } }

		public void OnColorAssetUpdated(ColorAsset ca)
		{
			if (this.colorAsset == ca) {
				UpdateProperty ();
			}
		}

	}
}