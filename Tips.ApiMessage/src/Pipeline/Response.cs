﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Tips.Pipeline
{
    public class Response<TItem> : Response
    {
        public Response() : this(Activator.CreateInstance<TItem>()) { }
        public Response(TItem item = default) => Item = item;
        public Response(Notification notification, TItem item = default)
        {
            Notifications.Add(notification);
            Item = item;
        }
        public Response(IEnumerable<Notification> notifications, TItem item = default)
        {
            Notifications.AddRange(notifications);
            Item = item;
        }

        public TItem Item { get; set; }
    }

    public class Response
    {
        public Response() {}
        public Response(Notification notification) => Notifications.Add(notification);
        public Response(IEnumerable<Notification> notifications) => Notifications.AddRange(notifications);

        public List<Notification> Notifications { get; } = new List<Notification>();

        public bool HasErrors() => Notifications.Any(notification => notification.Severity == Notification.SeverityType.Error);
        public bool IsNotFound() => Notifications.Any(notification => notification is NotFoundNotification);
    }
}
