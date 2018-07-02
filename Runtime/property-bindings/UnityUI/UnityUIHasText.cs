using UnityEngine.UI;

namespace BeatThat.Properties.UnityUI
{
    public class UnityUIHasText : HasText, IDrive<Text>
    {

        public Text driven { get { return this._text; } }

        public override bool sendsValueObjChanged { get { return false; } }

        override public string value { get { return this._text.text; } set { this._text.text = value; } }

        public object GetDrivenObject()
        {
            return this._text;
        }

        public bool ClearDriven()
        {
            m_text = null;
            return true;
        }

        private Text _text
        {
            get
            {
                if (m_text == null)
                {
                    m_text = GetComponent<Text>();
                }
                return m_text;
            }
        }

        private Text m_text;
    }
}
