using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace SimpleSiteMonitor
{
    class Monitor
    {
        static void Main()
        {
            var appSettings = System.Configuration.ConfigurationManager.AppSettings;
            
            foreach (var monitor in appSettings.AllKeys)
            {
                var urlAndText = appSettings[monitor].Split(',');
                var url = urlAndText[0];
                var textToLookFor = urlAndText[1];

                CheckUrl(url, textToLookFor);
            }
        }

        /// <summary>
        /// Makes a web request using the specified URL and verifies whether or not the request succeeds
        /// </summary>
        /// <param name="url">URL to check</param>
        /// <param name="textToLookFor">String of text to look for in HTMl response</param>
        private static void CheckUrl(string url, string textToLookFor)
        {
            HttpWebResponse response = null;
            string statusCode;
            string statusDescription;

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode != HttpStatusCode.OK)
                    return;

                var stream = response.GetResponseStream();
                if (stream == null)
                {
                    SendEmail("Site Monitor Error - response stream was null", String.Empty);
                    return;
                }

                var reader = new StreamReader(stream);
                var html = reader.ReadToEnd();

                if (html.ToLower().Contains(textToLookFor.ToLower()))
                    return;

                SendEmail("WEBSITE DOWN (expected text not found): " + url, 
                    "URL: " + url + Environment.NewLine 
                    + "The expected text \"" + textToLookFor + "\" was not found");
            }
            // WebException has an HttpWebResponse member which can provide an HTTP status code and description, so handle it differently
            catch (WebException webException)
            {
                response = (HttpWebResponse)webException.Response;

                if (response != null)
                {
                    statusCode = ((int)response.StatusCode).ToString(CultureInfo.InvariantCulture);
                    statusDescription = response.StatusDescription;
                }
                else
                {
                    statusCode = "-1";
                    statusDescription = webException.Message;
                }

                SendEmail("WEBSITE DOWN: " + url, 
                    "URL: " + url + Environment.NewLine 
                    + "Status Code: " + statusCode + Environment.NewLine 
                    + "Status Description: " + statusDescription);
            }
            catch (Exception generalException)
            {
                if (response != null)
                {
                    statusCode = ((int)response.StatusCode).ToString(CultureInfo.InvariantCulture);
                    statusDescription = response.StatusDescription;
                }
                else
                {
                    statusCode = "-1";
                    statusDescription = generalException.Message;
                }

                SendEmail("WEBSITE DOWN: " + url, 
                    "URL: " + url + Environment.NewLine 
                    + "Status Code: " + statusCode + Environment.NewLine 
                    + "Status Description: " + statusDescription);
            }
        }

        /// <summary>
        /// Sends an email using the SMTP settings in app.config
        /// </summary>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body</param>
        private static void SendEmail(string subject, string body)
        {
            var fromAddress = new MailAddress(Settings.Default.FromAddress);
            var fromPassword = Settings.Default.Password;
            var toAddresses = Settings.Default.ToAddress.Split(',');

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Host = Settings.Default.SmtpServer;
                smtpClient.Port = int.Parse(Settings.Default.SmtpPort);
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(fromAddress.Address, fromPassword);

                using (var message = new MailMessage())
                {
                    foreach (var address in toAddresses)
                        message.To.Add(address);

                    message.From = new MailAddress(Settings.Default.FromAddress);

                    message.Subject = subject;
                    message.Body = body;

                    smtpClient.Send(message);
                }
            }
        }
    }
}
