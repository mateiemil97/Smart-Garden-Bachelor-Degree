using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotification.Entities
{
    public class Notification
    {
        public string title { get; set; }
        public string body { get; set; }
        public string sound { get; set; }
        public string clickAction { get; set; }
        public string icon { get; set; }
    }
}
