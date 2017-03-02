using NotifManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Script.Serialization;

namespace NotifManager.Utility
{
    public static class OneSignalAPI
    {
        #region Message
        public static Message PostMessage(Message message)
        {
            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            request.Headers.Add("authorization", "Basic " + message.RestKey);

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            string timezone = "GMT-0300";

            string scheduledDate = message.ScheduleDate > DateTime.Now ? message.ScheduleDate.ToString("yyyy-MM-dd HH:mm:ss") + " " + timezone : "";

            object obj = new
            {
                app_id = message.AppId,
                contents = new { en = message.Content, pt = message.Content },
                headings = new { en = message.Title, pt = message.Title },
                subtitle = new { en = message.SubTitle, pt = message.SubTitle },
                url = message.Url,
                included_segments = new string[] { "All" },
                send_after = scheduledDate
            };

            string param = serializer.Serialize(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(param);

            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }

                string[] aux = responseContent.Split('"');

                Guid id;

                if (Guid.TryParse(aux[3], out id))
                    message.Id = id;
                else
                    message.Id = Guid.Empty;
            }
            catch (WebException ex)
            {
                responseContent = ex.Message;
            }

            message.Log = responseContent;

            return message;
        }

        public static MessageReply GetMessage(Guid messageId, Guid appId, string RestKey)
        {
            MessageReply result = new MessageReply();

            result.Id = messageId;

            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications/" + messageId.ToString() + "?app_id=" + appId) as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "Get";
            request.ContentType = "application/json; charset=utf-8";

            request.Headers.Add("authorization", "Basic " + RestKey);

            string responseContent = null;

            try
            {
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }

                string[] aux = responseContent.Split('"');

                result.Converteds = int.Parse(aux[60].Split(':')[1].Split(',')[0]);
                result.Sucessfuls = int.Parse(aux[136].Split(':')[1].Split(',')[0]);
                result.Faileds = int.Parse(aux[72].Split(':')[1].Split(',')[0]);
            }
            catch (WebException ex)
            {
                responseContent = ex.Message;
            }

            result.Log = responseContent;

            return result;
        }
        #endregion

        #region App
        public static App PostApp(App a)
        {
            var request = WebRequest.Create("https://onesignal.com/api/v1/apps") as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            request.Headers.Add("authorization", "Basic " + WebConfigurationManager.AppSettings["UserAuthKey"]);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            object obj = new
            {
                name = a.Name,
                chrome_web_origin = a.Url,
                chrome_web_default_notification_icon = a.Icon,
                chrome_web_sub_domain = a.SubDomain,
                safari_apns_p12 = "",
                safari_apns_p12_password = "",
                site_name = a.Url,
                safari_site_origin = a.Url,
                safari_icon_256_256 = a.Icon
            };
            string param = serializer.Serialize(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(param);

            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }

                string[] aux = responseContent.Split('"');

                a.Id = Guid.Parse(aux[3]);
                a.SafariId = Guid.Parse(aux[47].Split('.')[3]);
                a.RestKey = aux[87];
            }
            catch (WebException ex)
            {
                responseContent = ex.Message;
            }

            a.Log = responseContent;

            return a;
        }

        public static App PutApp(App a)
        {
            var request = WebRequest.Create("https://onesignal.com/api/v1/apps/" + a.Id.ToString()) as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "PUT";
            request.ContentType = "application/json; charset=utf-8";

            request.Headers.Add("authorization", "Basic " + WebConfigurationManager.AppSettings["UserAuthKey"]);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            object obj = new
            {
                name = a.Name,
                chrome_web_origin = a.Url,
                chrome_web_default_notification_icon = a.Icon,
                chrome_web_sub_domain = a.SubDomain,
                safari_apns_p12 = "",
                safari_apns_p12_password = "",
                site_name = a.Url,
                safari_site_origin = a.Url,
                safari_icon_256_256 = a.Icon
            };
            string param = serializer.Serialize(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(param);

            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }

                string[] aux = responseContent.Split('"');
            }
            catch (WebException ex)
            {
                responseContent = ex.Message;
            }

            a.Log = responseContent;

            return a;
        }
        #endregion
    }
}