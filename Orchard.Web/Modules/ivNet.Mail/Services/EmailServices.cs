
using System.Net;
using System.Net.Mail;
using System.Web.Configuration;
using ivNet.Mail.Helpers;
using Orchard;
using Orchard.Email.Models;
using Orchard.ContentManagement;
using Orchard.Security;
using System;
using System.Text.RegularExpressions;
using System.Web;

namespace ivNet.Mail.Services
{
    public interface IEmailServices : IDependency
    {
        void SendActivationEmail(string email, string password, string subject);       
    }

    public class EmailServices : BaseService, IEmailServices
    {
        private readonly IOrchardServices _orchardServices;

        public EmailServices(IAuthenticationService authenticationService,
            IOrchardServices orchardServices) 
            : base(authenticationService)
        {
            _orchardServices = orchardServices;
        }

        public void SendActivationEmail(string email, string password, string subject)
        {           
            var key = string.Format("{0}|{1}", email, password).ToLowerInvariant();         
            var eMailLink = CreateEmailLink(key);
            var eMailMessage =
                string.Format("<p>Please click on the <a href='{0}' target='blank'>this link</a> in order to activate you registration. Thanks you.</p>", eMailLink);

             SendMessage(email, subject, eMailMessage);
        }

        private static string CreateEmailLink(string ownerId)
        {
            var encodedOwnerId = HttpUtility.UrlEncode(QueryStringHelper.Base64ForUrlEncode(ownerId));
            return string.Format("{0}owner/activation/{1}", GetBaseUrl(), encodedOwnerId);
        }

        private static string GetBaseUrl()
        {
            var request = HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (!string.IsNullOrWhiteSpace(appUrl))
            {
                if (appUrl.LastIndexOf("/", StringComparison.Ordinal) != appUrl.Length - 1)
                    appUrl += "/";
            }

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }

        private void SendMessage(string toEmail, string subject, string message)
        {
            var smtpSettings = _orchardServices.WorkContext.CurrentSite.As<SmtpSettingsPart>();

            // can't process emails if the Smtp settings have not yet been set
            if (smtpSettings == null || !smtpSettings.IsValid())
            {
                throw new Exception("Site SMTP settings not configured");
            }

            using (var smtpClient = new SmtpClient())
            {            
                smtpClient.UseDefaultCredentials = !smtpSettings.RequireCredentials;
                smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;

                if (smtpSettings.Host != null)
                    smtpClient.Host = smtpSettings.Host;

                smtpClient.Port = smtpSettings.Port;
                smtpClient.EnableSsl = smtpSettings.EnableSsl;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                var mailMessage = new MailMessage();

                try
                {
                    mailMessage = new MailMessage
                    {
                        Subject = string.Format("{0}", subject),
                        Body = message,
                        From = new MailAddress(smtpSettings.Address)
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("ivNet.Mail: Bad eMail message [{0}].", ex.Message));
                }

                var webConfig =
                WebConfigurationManager.OpenWebConfiguration("~");
                if (webConfig.AppSettings.Settings.Count > 0)
                {
                    var customSetting =
                       webConfig.AppSettings.Settings["ivNetEmail"];

                    if (customSetting!=null && !string.IsNullOrEmpty(customSetting.Value))
                        mailMessage.Bcc.Add(new MailAddress(customSetting.Value));
                                                             
                }

                mailMessage.To.Add(new MailAddress(toEmail));
                mailMessage.IsBodyHtml = mailMessage.Body.Contains("<") && mailMessage.Body.Contains(">");

                try
                {
                    smtpClient.Send(mailMessage);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("ivNet.Mail: Failed to send eMail from [{0}] to [{1}]({2}) - {3}.", mailMessage.From.Address, mailMessage.To[0].Address, mailMessage.To.Count, ex.Message));
                }

            }
        }       
    }
}