using UnityEngine;
using UnityEngine.UI;

namespace BeatThat.Properties
{
    [RequireComponent(typeof(Image))]
	public class HasTextureSprite : HasTexture
    {
        Image imgCmp { get { return this.GetComponent<Image>(); } }

		new public Sprite value
		{
			get { return GetSprite(); } 
			set {
				if(GetSprite() == value) {
					return;
				}
                SetSprite(value);
				if(onValueChanged != null) {
					onValueChanged.Invoke();
				}
			}
		}

        public override bool sendsValueObjChanged
        {
            get
            {
                return false;
            }
        }

        override protected Texture GetTexture()
        {
            return null;
        }

        Sprite GetSprite()
        {
            return imgCmp.sprite;
        }

        override protected void SetTexture(Texture t)
        {
        }

        void SetSprite(Sprite s)
        {
            imgCmp.sprite = s;
        }
	}
}
