using BeatThat.TransformPathExt;
using UnityEngine.Events;
using UnityEngine;
using BeatThat.UnityEvents;

namespace BeatThat.Properties
{
    public interface ITextureProp : IHasProp<Texture> { }

    /// <summary>
    /// A HasTexture that sends events when the value changes
    /// </summary>
    public abstract class TextureProp : HasTexture, ITextureProp
    {
        public bool m_debug;
        public bool m_debugBreakOnSetValue;

        [Tooltip("the prop will resume this value OnDisable (when resetDefaultValueOnDisable is set)")]
        public Texture m_resetValue;

        [Tooltip("set as 'BindToProperty' to use another property to drive the value of this property; set as 'DriveProperty' to use this property to drive the value of another property")]
        [HideInInspector] public BindOrDrivePropertyOptions m_bindOrDrivePropertyOptions;
        public BindOrDrivePropertyOptions bindOrDrivePropertyOptions { get { return m_bindOrDrivePropertyOptions; } set { m_bindOrDrivePropertyOptions = value; } }
        [HideInInspector] public TextureProp m_bindToProperty;
        [HideInInspector] public TextureProp m_driveProperty;
        private bool hasConnectedBinding;


        virtual protected void OnEnable()
        {
            // NOTE: this needs to be OnEnable (not awake or start)
            // because otherwise if this component exists on, say,
            // a pooled object that's getting enabled and disabled, 
            // the Connect logic below will ONLY execute on first use of object.

            if (this.bindOrDrivePropertyOptions == BindOrDrivePropertyOptions.Disabled)
            {
#if UNITY_EDITOR || DEBUG_UNSTRIP
                if (m_debug)
                {
                    Debug.Log("[" + this.Path() + "] " + GetType() + " bind or drive property is disabled");
                }
#endif
                return;
            }

            if (this.hasConnectedBinding && this.bindOrDrivePropertyOptions == BindOrDrivePropertyOptions.BindToProperty && m_bindToProperty != null)
            {
#if UNITY_EDITOR || DEBUG_UNSTRIP
                if (m_debug)
                {
                    Debug.Log("[" + this.Path() + "] " + GetType() + " bind or drive property is already connected. Setting value to " + m_bindToProperty.value);
                }
#endif
                SetValue(m_bindToProperty.value);
                return;
            }

#if UNITY_EDITOR || DEBUG_UNSTRIP
            if (m_debug)
            {
                Debug.Log("[" + this.Path() + "] " + GetType() + " will attempt connection with bind opts " + this.bindOrDrivePropertyOptions
                      + " and bind-to-prop=" + m_bindToProperty.Path() + ", and drive-prop=" + m_driveProperty.Path());
            }
#endif

            this.hasConnectedBinding |=
                this.bindOrDrivePropertyOptions == BindOrDrivePropertyOptions.BindToProperty
                && m_bindToProperty != null
                && BindTexture2Texture.Connect<BindTexture2Texture>(m_bindToProperty, this, m_resetValue, m_debug);

            this.hasConnectedBinding |=
                this.bindOrDrivePropertyOptions == BindOrDrivePropertyOptions.DriveProperty
                && m_driveProperty != null
                && BindTexture2Texture.Connect<BindTexture2Texture>(this, m_driveProperty, m_resetValue, m_debug);


#if UNITY_EDITOR || DEBUG_UNSTRIP
            if (!this.hasConnectedBinding)
            {
                Debug.LogWarning("[" + Time.frameCount + "] failed to bind to prop at " + GetType() + "[" + this.Path()
            + "]. If you don't want property binding on this component, set its BindOrDriveProperty field to Disabled");
            }
#endif
        }


        public UnityEvent<Texture> onValueChanged
        {
            get { return m_onValueChanged ?? (m_onValueChanged = new TextureEvent()); }
            set { m_onValueChanged = value; }
        }
        [SerializeField] protected UnityEvent<Texture> m_onValueChanged;

        override public Texture value
        {
            get { return GetValue(); }
            set { SetValue(value); }
        }

        override public object valueObj { get { return this.value; } }

        abstract protected Texture GetValue();

        /// <summary>
        /// Override for case where you might want to store a value even if the value currently returned matches the new value set.
        /// This is the case for working with Animator params.
        /// </summary>
        virtual protected void EnsureValue(Texture s) { }

        abstract protected void _SetValue(Texture s);

        override public bool sendsValueObjChanged { get { return true; } }

        protected void SendValueChanged(Texture val)
        {
            SendValueObjChanged();
            if (m_onValueChanged != null)
            {
                m_onValueChanged.Invoke(val);
            }
        }

        public void SetValue(Texture val, PropertyEventOptions opts = PropertyEventOptions.SendOnChange)
        {
#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
            if (m_debug)
            {
                Debug.Log("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::set_value to " + val);
            }
#endif

            if (opts != PropertyEventOptions.Force && val == GetValue())
            {
                EnsureValue(val);
                return;
            }

            _SetValue(val);

#if UNITY_EDITOR
            if (m_debugBreakOnSetValue)
            {
                Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::set_value to " + val + " BREAK ON SET VALUE is enabled");
                Debug.Break();
            }
#endif

            if (opts != PropertyEventOptions.Disable)
            {
                SendValueChanged(val);
            }
        }

    }

}

