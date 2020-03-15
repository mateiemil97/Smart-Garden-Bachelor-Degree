using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotification.Models
{
    public class NotificationDto
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string To { get; set; }
    }
}
