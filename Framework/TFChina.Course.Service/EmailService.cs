using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFChina.Course.Service
{
    /// <summary>
    /// 邮件服务
    /// </summary>
    public class EmailService
    {
        public readonly SmtpProvider _smtpProvider;

        public EmailService()
        {
            _smtpProvider = new SmtpProvider();
        }

        /// <summary>
        /// send e-mail
        /// </summary>
        public void Send(Message message)
        {
            _smtpProvider.Send(message);
        }
    }
}
