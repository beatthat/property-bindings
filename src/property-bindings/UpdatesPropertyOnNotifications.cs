using BeatThat;
using System;
using System.Collections.Generic;

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
					Bind (n, this.UpdateProperty);
				}
			}
        }

        private Action updateAction {  get { return m_updateAction != null ? m_updateAction : (m_updateAction = this.UpdateProperty);  } }
        private Action m_updateAction;

        private void UpdateProperty()
        {
            this.property.value = this.authoritativeValue;
        }
    }
}

