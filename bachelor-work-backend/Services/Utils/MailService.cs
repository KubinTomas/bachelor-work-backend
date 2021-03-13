using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using bachelor_work_backend.Database;
using Microsoft.Extensions.Configuration;

namespace bachelor_work_backend.Services.Utils
{
    public class MailService
    {
        public IConfiguration Configuration { get; private set; }

        public MailService(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void SendKickFromActionMail(string mailTo, BlockAction action)
        {
            MailMessage message = new MailMessage();
            var smtp = GetSmpt();
            message.From = new MailAddress(GetMailFrom());
            message.To.Add(new MailAddress(mailTo));

            message.Subject = "Zrušení akce: " + action.Name + ", konající se " + action.StartDate.ToString("d.M.yyyy H:m");
            message.IsBodyHtml = true; //to make message body as html  
            message.Body = "Dobrý den, akce na kterou jste přihlášen byla zrušena.";

            smtp.Send(message);
        }

        public void SendKickFromActionQueueMail(string mailTo, BlockAction action)
        {
            MailMessage message = new MailMessage();
            var smtp = GetSmpt();
            message.From = new MailAddress(GetMailFrom());
            message.To.Add(new MailAddress(mailTo));

            message.Subject = "Zrušení akce: " + action.Name + ", konající se " + action.StartDate.ToString("d.M.yyyy H:m");
            message.IsBodyHtml = true; //to make message body as html  
            message.Body = "Dobrý den, akce na kterou jste přihlášen byla zrušena. Došlo tedy k uvolnění z fronty čekatelů.";

            smtp.Send(message);
        }

        public void SendPasswordRecoveryMail(string mailTo, string userGuid, DateTime validUntil)
        {
            MailMessage message = new MailMessage();
            var smtp = GetSmpt();
            message.From = new MailAddress(GetMailFrom());
            message.To.Add(new MailAddress(mailTo));

            message.Subject = "Obnovení hesla";
            message.IsBodyHtml = true; //to make message body as html  
            message.Body = "Dobrý den, byla zaznamenáná žádost o obnovení hesla. Pro změnu hesla klikněte na následující odkaz. Odkaz je platný do "
                + validUntil.ToString("d.M.yyyy H:m") + "." 
                + "<br>" + GetPasswordRecoverUrl(userGuid);

            smtp.Send(message);
        }
        public void SendConfirmationMail(string mailTo, string userGuid)
        {
            MailMessage message = new MailMessage();
            var smtp = GetSmpt();
            message.From = new MailAddress(GetMailFrom());
            message.To.Add(new MailAddress(mailTo));

            message.Subject = "Potvzení uživatelského konta";
            message.IsBodyHtml = true; //to make message body as html  
            message.Body = "Dobrý den, Váš účet byl zaregistrován v systému. Prosím potvrďte kliknutím na následující odkaz. <br>" + GetConfrimUrl(userGuid);

            smtp.Send(message);
        }

        private SmtpClient GetSmpt()
        {
            SmtpClient smtp = new SmtpClient();

            smtp.Port = 587;
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = GetNetworkCredential();
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            return smtp;
        }

        private string GetConfrimUrl(string userGuid)
        {
            return Configuration.GetSection("Mail").GetValue<string>("ConfrimUrl") + "/" + userGuid;
        }

        private string GetPasswordRecoverUrl(string userGuid)
        {
            return Configuration.GetSection("Mail").GetValue<string>("PasswordRecoverUrl") + "/" + userGuid;
        }

        private string GetMailFrom()
        {
            return Configuration.GetSection("Mail").GetValue<string>("MailFromAddress");
        }

        private NetworkCredential GetNetworkCredential()
        {
            var password = Configuration.GetSection("Mail").GetValue<string>("MailFromAddressPassword");

            return new NetworkCredential(GetMailFrom(), password);
        }
    }
}
