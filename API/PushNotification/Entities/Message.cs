using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotification.Entities
{
    public class Message
    {
        public string to { get; set; }
        public Notification notification { get; set; }
        public string priority { get; set; }
    }
}
