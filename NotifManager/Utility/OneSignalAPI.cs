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
            object obj;

            if (!a.IsHttps)
            {
                obj = new
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
            }
            else
            {
                obj = new
                {
                    name = a.Name,
                    chrome_web_origin = a.Url,
                    chrome_web_default_notification_icon = a.Icon,
                    safari_apns_p12 = "",
                    safari_apns_p12_password = "",
                    site_name = a.Url,
                    safari_site_origin = a.Url,
                    safari_icon_256_256 = a.Icon
                };
            }
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

        #region Device
        public static List<Device> ListDevice(Guid appId, string RestKey)
        {
            List<Device> result = new List<Device>();

            var request = WebRequest.Create("https://onesignal.com/api/v1/players?app_id=" + appId + "&limit=300&offset=0") as HttpWebRequest;

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


                Device dev = new Device();
                string[] device, aux = responseContent.Split(new string[] { "\"players\":[" }, StringSplitOptions.None);

                aux = aux[1].Split('{');

                for (int i = 1; i < aux.Length; i += 2)
                {
                    device = (aux[i] + aux[i + 1]).Split('"');

                    long miliseconds;

                    dev.Id = new Guid(device[3]);
                    dev.SessionCount = int.Parse(device[10].Replace(":", "").Replace(",", ""));
                    dev.DeviceOS = device[21];
                    dev.DeviceModel = device[27];

                    miliseconds = long.Parse(device[40].Replace(":", "").Replace(",", ""));
                    dev.CreatedDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    dev.CreatedDate = dev.CreatedDate.AddSeconds(miliseconds).ToLocalTime();

                    miliseconds = long.Parse(device[34].Replace(":", "").Replace(",", ""));
                    dev.LastActive = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    dev.LastActive = dev.LastActive.AddSeconds(miliseconds).ToLocalTime();

                    switch (device[24])
                    {
                        case ":0,":
                            dev.DeviceType = "iOS";
                            break;
                        case ":1,":
                            dev.DeviceType = "ANDROID";
                            break;
                        case ":2,":
                            dev.DeviceType = "AMAZON";
                            break;
                        case ":3,":
                            dev.DeviceType = "WINDOWSPHONE (MPNS)";
                            break;
                        case ":4,":
                            dev.DeviceType = "CHROME APPS / EXTENSIONS";
                            break;
                        case ":5,":
                            dev.DeviceType = "CHROME WEB PUSH";
                            break;
                        case ":6,":
                            dev.DeviceType = "WINDOWSPHONE (WNS)";
                            break;
                        case ":7,":
                            dev.DeviceType = "SAFARI";
                            break;
                        case ":8,":
                            dev.DeviceType = "FIREFOX";
                            break;
                        case ":9,":
                            dev.DeviceType = "MACOS";
                            break;
                    }

                    result.Add(dev);
                    dev = new Device();
                }
            }
            catch (WebException ex)
            {
                responseContent = ex.Message;
            }

            return result;
        }
        #endregion

        #region CSV
        public static string GenerateCSV(Guid appId, string restKey)
        {
            string result = "";

            var request = WebRequest.Create("https://onesignal.com/api/v1/players/csv_export?app_id=" + appId) as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "Post";
            request.ContentType = "application/json; charset=utf-8";

            request.Headers.Add("authorization", "Basic " + restKey);

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

                if (aux.Length > 0)
                    result = aux[3];
            }
            catch (WebException ex)
            {
                responseContent = ex.Message;
                result = "";
            }

            return result;
        }
        #endregion

        public static string GenerateScript(App app)
        {
            string result = "";

            result += "<script src=\"https:/" + "/cdn.onesignal.com/sdks/OneSignalSDK.js\"></script>\r\n";
            result += "<script>\r\n";
            result += "var OneSignal = window.OneSignal || [];\r\n";
            result += "OneSignal.push([\"init\", {\r\n";
            result += "appId: \"" + app.Id + "\",\r\n";
            result += "autoRegister: true,\r\n";
            if (!app.IsHttps)
                result += "subdomainName: '" + app.SubDomain + "',\r\n";
            result += "httpPermissionRequest:\r\n";
            result += "{\r\n";
            result += "enable: true\r\n";
            result += "},\r\n";
            result += "notifyButton:\r\n";
            result += "{\r\n";
            result += "enable: true\r\n";
            result += "},\r\n";
            result += "promptOptions:\r\n";
            result += "{\r\n";
            result += "siteName: '" + app.Name + "',\r\n";
            result += "actionMessage: \"Nos permita mostrar notificações sobre o nosso conteúdo\",\r\n";
            result += "exampleNotificationTitle: 'Título',\r\n";
            result += "exampleNotificationMessage: 'Mensagem',\r\n";
            result += "exampleNotificationCaption: 'Você pode cancelar a inscrição a qualquer momento.',\r\n";
            result += "acceptButtonText: \"Permitir\",\r\n";
            result += "cancelButtonText: \"Não Obrigado\",\r\n";
            result += "safari_web_id: '" + app.SafariId + "'\r\n";
            result += "},\r\n";
            result += "welcomeNotification:\r\n";
            result += "{\r\n";
            result += "disable: false,\r\n";
            result += "title: 'Bem-vindo',\r\n";
            result += "message: 'Obrigado por se inscrever.'\r\n";
            result += "},\r\n";
            result += "httpPermissionRequest:\r\n";
            result += "{\r\n";
            result += "enable: true,\r\n";
            result += "modalTitle: 'Fique por dentro',\r\n";
            result += "modalMessage: 'Seja alertado sempre que algo grande aconteça.',\r\n";
            result += "modalButtonText: 'Eu quero saber mais!'\r\n";
            result += "}\r\n";
            result += "}\r\n";
            result += "]);\r\n";
            result += "</script>\r\n";

            return result;
        }
    }
}