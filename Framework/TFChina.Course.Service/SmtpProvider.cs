using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TFChina.Course.Data;

namespace TFChina.Course.Service
{
    public class SmtpProvider
    {
        public bool Send(Message message)
        {
            var result = false;
            try
            {
                var userName = ConfigurationManager.AppSettings["Smtp:UserName"];
                var password = ConfigurationManager.AppSettings["Smtp:Password"];
                var host = ConfigurationManager.AppSettings["Smtp:Host"];
                var port = int.Parse(ConfigurationManager.AppSettings["Smtp:Port"]);
                message.From = ConfigurationManager.AppSettings["Smtp:From"];
                var displayName = ConfigurationManager.AppSettings["Smtp:DisplayName"];
               
                var msg = new MailMessage();
                var fromAddress = new MailAddress(message.From, displayName, Encoding.UTF8);
                msg.From = fromAddress;
                msg.To.Add(message.To);
                msg.Subject = message.Subject;
                msg.Body = message.Content;
                msg.IsBodyHtml = true;
                msg.Priority = MailPriority.High;
                var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(userName, password),
                    //EnableSsl = true;
                };
                client.Send(msg);
                client.Dispose();
                result = true;
            }
            catch (Exception e)
            {
                message.ErrorMsg = e.Message;
                throw;
            }
            finally
            {

            }
            return result;
        }
    }
}
