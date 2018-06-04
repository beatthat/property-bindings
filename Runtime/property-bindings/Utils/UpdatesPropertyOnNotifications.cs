using BeatThat;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeatThat
{
    /// <summary>
    /// Keeps a property in sync with an authoritative source for a setup with two features:
    /// 
    /// 1) the authoritative value of the property can be retrieved at any time
    /// 2) the property only needs to be updated when one of a specific set of Notifications is sent
    /// </summary>
    public abstract class UpdatesPropertyOnNotifications<PropertyType, ValueType> : PropertyBinding<PropertyType>
        where PropertyType : class, IHasValue<ValueType>
    {
        public bool m_debug;

        /// <summary>
        /// Override to provide the authoritative value for the property,
        /// e.g. Services.Require<MyService>.myProperty
        /// </summary>
        abstract protected ValueType authoritativeValue { get; }

        /// <summary>
        /// Override to provide the Notifications that gets sent when this property has been (or may have been) updated
        /// e.g. MyNotifications.UPDATED
        /// </summary>
		abstract protected void GetUpdateNotifications(ICollection<string> notifications);

        protected override void BindProperty()
        {
            UpdateProperty();
			using (var ns = ListPool<string>.Get ()) {
				GetUpdateNotifications (ns);
				foreach (var n in ns) {
                    Bind (n, this.updateAction);
				}
			}
        }

        private Action updateAction {  get { return m_updateAction != null ? m_updateAction : (m_updateAction = this.UpdateProperty);  } }
        private Action m_updateAction;

        protected void UpdateProperty()
        {
#if UNITY_EDITOR || DEBUG_UNSTRIP
            if (m_debug)
            {
                Debug.Log("[" + Time.frameCount + "][" + this.Path() + "] " + GetType()
                          + " - updating from " + this.property.value
                          + "  to " + this.authoritativeValue);
            }
#endif
            this.property.value = this.authoritativeValue;
        }
    }
}

