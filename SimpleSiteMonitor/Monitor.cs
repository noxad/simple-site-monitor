using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace SimpleSiteMonitor
{
    class Monitor
    {
        static void Main(string[] args)
        {
            System.Collections.Specialized.NameValueCollection appSettings = System.Configuration.ConfigurationManager.AppSettings;
            
            foreach (string monitor in appSettings.AllKeys)
            {
                string[] urlAndText = appSettings[monitor].Split(',');
                string url = urlAndText[0];
                string textToLookFor = urlAndText[1];

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
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string html = reader.ReadToEnd();

                    if (html.ToLower().Contains(textToLookFor.ToLower()))
                    {
                        return;
                    }
                    else
                    {
                        SendEmail("WEBSITE DOWN (expected text not found): " + url, "URL: " + url + Environment.NewLine
                            + "The expected text \"" + textToLookFor + "\" was not found");
                    }                    
                }

                statusCode = ((int)response.StatusCode).ToString();
                statusDescription = response.StatusDescription;
            }
            // WebException has an HttpWebResponse member which can provide an HTTP status code and description, so handle it differently
            catch (WebException webException)
            {
                response = (HttpWebResponse)webException.Response;

                if (response != null)
                {
                    statusCode = ((int)response.StatusCode).ToString();
                    statusDescription = response.StatusDescription;
                }
                else
                {
                    statusCode = "-1";
                    statusDescription = webException.Message;
                }

                SendEmail("WEBSITE DOWN: " + url, "URL: " + url + Environment.NewLine
                    + "Status Code: " + statusCode + Environment.NewLine
                    + "Status Description: " + statusDescription);
            }
            catch (Exception generalException)
            {
                if (response != null)
                {
                    statusCode = ((int)response.StatusCode).ToString();
                    statusDescription = response.StatusDescription;
                }
                else
                {
                    statusCode = "-1";
                    statusDescription = generalException.Message;
                }

                SendEmail("WEBSITE DOWN: " + url, "URL: " + url + Environment.NewLine
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
            MailAddress fromAddress = new MailAddress(Settings.Default.FromAddress);
            string fromPassword = Settings.Default.Password;
            MailAddress toAddress = new MailAddress(Settings.Default.ToAddress);            

            using (SmtpClient smtp = new SmtpClient())
            {
                smtp.Host = Settings.Default.SmtpServer;
                smtp.Port = int.Parse(Settings.Default.SmtpPort);
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(fromAddress.Address, fromPassword);

                using (MailMessage message = new MailMessage(fromAddress, toAddress))
                {
                    message.Subject = subject;
                    message.Body = body;

                    smtp.Send(message);
                }
            }
        }
    }
}
