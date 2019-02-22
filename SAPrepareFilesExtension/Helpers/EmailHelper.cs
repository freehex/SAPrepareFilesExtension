using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace SAPrepareFilesExtension.Helpers
{
    public static class EmailHelper
    {
        public static void SendMessage(string title, string body, IEnumerable<string> filesPath)
        {
            LogHelper.Begin(new { title, body, filesPath });

            var emailAddressFrom = string.IsNullOrWhiteSpace(GeneralSettings.Default.EmailAddressFrom) ? UserPrincipal.Current.EmailAddress : GeneralSettings.Default.EmailAddressFrom;

            MailMessage emailMessage = new MailMessage(
                emailAddressFrom, 
                GeneralSettings.Default.EmailAddressTo
            );

            emailMessage.IsBodyHtml = false;
            emailMessage.Subject = SettingsHelper.GetValue(title);
            emailMessage.BodyEncoding = Encoding.Unicode;
            emailMessage.HeadersEncoding = Encoding.Unicode;
            emailMessage.SubjectEncoding = Encoding.Unicode;
            emailMessage.Body = body;

            emailMessage.Headers.Add("X-Unsent", "1"); //to activate send mode in the Outlook

            foreach (var filePath in filesPath)
                emailMessage.Attachments.Add(new Attachment(filePath));

            LogHelper.Trace(new { emailAddressFrom, GeneralSettings.Default.EmailAddressTo, emailMessage.Subject });

            // workaround for pass email with multiple attachments to the Outlook:
            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            client.PickupDirectoryLocation = Path.GetDirectoryName(filesPath.ElementAt(0));
            client.Send(emailMessage);

            var emailFilePath = FileHelper.GetEmailFilePath(filesPath.ElementAt(0));

            LogHelper.Trace(new { SmtpDeliveryMethod.SpecifiedPickupDirectory, emailFilePath });

            Process.Start(emailFilePath);

            //Thread.Sleep(1000);
            //File.Delete(emailFilePath);
            LogHelper.End();
        }
    }
}
