using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PushNotification.Entities;
using Microsoft.AspNetCore.Mvc;
using PushNotification.Models;

namespace PushNotification.Controller
{

    public class PushNotificationController: Microsoft.AspNetCore.Mvc.Controller
    {
        private static Uri FireBasePushNotificationsURL = new Uri("https://fcm.googleapis.com/fcm/send");
        private const string ServerKey = "AAAAxTImvP4:APA91bFyqLUO66kZitgRu36J62qmHjh4JaCTUyqRl9L6Jw1qH78buNQR-NLSeRM3sxHiL-oDCRflERxYWZdr5Ro_c1kgZLjg9LIQDzi0PBpdGkmPVNolnwYsmn4bAJhrWXbBXJrszXi5";
        private IHttpClientFactory _clientFactory;

        public PushNotificationController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceTokens">List of all devices assigned to a user</param>
        /// <param name="title">Title of notification</param>
        /// <param name="body">Description of notification</param>
        /// <param name="data">Object with all extra information you want to send hidden in the notification</param>
        /// <returns></returns>
        [HttpPost("api/push-notification")]
        public async Task<bool> SendPushNotification([FromBody]NotificationDto notification)
        {
            bool sent = false;
            if (notification.To != null)
            {
                //Object creation
                var messageInformation = new Message()
                {
                    notification = new Notification()
                    {
                        title = "Smart drop",
                        body = notification.Body,
                        sound = "default",
                        clickAction = "FCM_PLUGIN_ACTIVITY",
                        icon = "fcm_push_icon"

                    },
                    to = notification.To,
                    priority = "high"
                };

                //Object to JSON STRUCTURE => using Newtonsoft.Json;
                string jsonMessage = JsonConvert.SerializeObject(messageInformation);

           
                //Create request to firebase API

                var request = new HttpRequestMessage(HttpMethod.Post, FireBasePushNotificationsURL);

                request.Headers.TryAddWithoutValidation("Authorization", "key=" + ServerKey);
                request.Content = new StringContent(jsonMessage,Encoding.UTF8,"application/json");

                HttpResponseMessage result;


                using (var client = new HttpClient())
                {
                    result = await client.SendAsync(request);
                    sent = result.IsSuccessStatusCode;
                }
            }

            return sent;
        }
        
    }

}
