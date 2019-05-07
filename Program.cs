using System;
using System.Net.Mail;
using System.Text;

namespace OPM_Notification
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.Port = 587;
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("joshwhitfield9919@gmail.com", "jo@GmAiL@9919");

                MailMessage mm = new MailMessage("joshwhitfield9919@gmail.com", "joshwhitfield9919@gmail.com", "test", "test");
                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                client.Send(mm);
                Console.WriteLine("Email sent");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}
