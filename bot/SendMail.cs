using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Mail;
using System.Net;

namespace bot
{
    class SendMail
    {

        public void SendEmailKeIT(string Subject, string msg)
        {
            string email_it = "xx";
            string password_it = "xxx";

            string email_it_1 = "xxx";
            string email_it_2 = "xxx";


            Console.WriteLine("Mail To IT");
            MailAddress to = new MailAddress(email_it_1.ToString());

            Console.WriteLine("Mail From Gmail ");
            MailAddress from = new MailAddress(email_it.ToString());

            MailMessage mail = new MailMessage(email_it.ToString(), email_it_1.ToString());
            mail.CC.Add(email_it_2.ToString());
            mail.Subject = Subject.ToString();
            mail.Body = msg.ToString();

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;

            smtp.Credentials = new NetworkCredential(email_it.ToString(), password_it.ToString());
            smtp.EnableSsl = true;
            Console.WriteLine("Sending email...");
            try
            {
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.CreateLog("error-sendmail", ex.StackTrace, ex.Message + " | " + msg + " | " + Subject, null);
            }
        }
    }
}
