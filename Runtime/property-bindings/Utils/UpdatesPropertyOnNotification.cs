using BeatThat;
using System;
using System.Collections.Generic;

namespace BeatThat
{
    /// <summary>
    /// Keeps a property in sync with an authoritative source for a setup with two features:
    /// 
    /// 1) the authoritative value of the property can be retrieved at any time
    /// 2) the property only needs to be updated when a specific Notification is sent
    /// </summary>
	public abstract class UpdatesPropertyOnNotification<PropertyType, ValueType> : UpdatesPropertyOnNotifications<PropertyType, ValueType>
        where PropertyType : class, IHasValue<ValueType>
    {

        /// <summary>
        /// Override to provide the Notification that gets sent when this property has been (or may have been) updated
        /// e.g. MyNotifications.UPDATED
        /// </summary>
        abstract protected string updateNotification { get; }

		sealed override protected void GetUpdateNotifications(ICollection<string> notifications)
		{
			notifications.Add(this.updateNotification);
		}

    }
}

