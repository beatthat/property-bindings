using UnityEngine;

namespace BeatThat
{
	public class BindColorToColor : BindPropToProp<ColorProp, IHasColor, Color>, IHasColorAsset
	{
		public bool m_useAssetForDefaultValue;
		public ColorAsset m_defaultValueAsset;

		override protected Color defaultValue 
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