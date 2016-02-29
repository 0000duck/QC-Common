using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using QuantumConcepts.Common.Extensions;

namespace QuantumConcepts.Common.Utils
{
    public class EmailUtil
    {
        public static void SendEmail(string from, string to, string subject, string body, bool isBodyHtml = false, params Attachment[] attachments)
        {
            SendEmail(from, new[] { to }, subject, body, isBodyHtml, attachments);
        }

        public static void SendEmail(string from, IEnumerable<string> to, string subject, string body, bool isBodyHtml = false, params Attachment[] attachments)
        {
            SmtpClient client = new SmtpClient();
            MailMessage message = new MailMessage();

            if (!from.IsNullOrEmpty())
                message.ReplyToList.Add(new MailAddress(from));

            if (!ConfigUtil.Instance.OverrideEmailAddress.IsNullOrEmpty())
                message.To.Add(ConfigUtil.Instance.OverrideEmailAddress);
            else
                foreach (string s in to)
                    message.To.Add(s);

            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = isBodyHtml;

            if (attachments != null)
                foreach (Attachment attachment in attachments)
                    message.Attachments.Add(attachment);

            client.Send(message);
        }
    }
}
