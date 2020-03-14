using System;
using System.Collections.Generic;
using System.Text;

namespace FirebasePushNotification.Entities
{
    public class Message
    {   public string Registration_ids { get; set; }
        public Notification Notification { get; set; }
        public object Data { get; set; }
    }
}
