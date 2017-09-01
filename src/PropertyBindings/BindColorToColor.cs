using UnityEngine;

namespace BeatThat
{
	public class BindColorToColor : BindPropToProp<ColorProp, IHasColor, Color> 
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


	}
}