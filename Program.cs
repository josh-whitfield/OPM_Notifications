using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using HtmlAgilityPack;

namespace OPM_Notification
{
    public class DataStore
    {
        public string FullName { get; set; }
        public decimal Issue { get; set; }
        public DateTime DateFound { get; set; }
        public string Link { get; set; }
        public DataStore()
        {
            FullName = "";
            Issue = 0;
            DateFound = new DateTime();
            Link = "";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"C:\Users\Josh\source\repos\OPM_Notification\OPM_Database.csv";

            List<string> issueList = new List<string>();
            using (var streamReader = new StreamReader(filePath))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    var value = line.Split(',');

                    issueList.Add(value[1]);
                }
            }

            decimal latestIssue = Convert.ToDecimal(issueList[issueList.Count - 1]);

            DataStore OPM_Data = new DataStore();
            OPM_Data = SearchWebsite();

            if (OPM_Data.Issue > latestIssue)
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write))
                {
                    using (var streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.WriteLine(string.Format("{0},{1},{2},{3}", OPM_Data.FullName, OPM_Data.Issue.ToString(), OPM_Data.Link, OPM_Data.DateFound.ToString()));
                    }
                }

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

                    MailMessage mm = new MailMessage(
                                                         "joshwhitfield9919@gmail.com"
                                                        , "joshwhitfield9919@gmail.com"
                                                        , "New One Punch Man Manga"
                                                        , "One Punch Man issue #" + OPM_Data.Issue + " is now available.\r\n"
                                                        + "It is located here: " + OPM_Data.Link);
                    mm.BodyEncoding = UTF8Encoding.UTF8;
                    mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                    client.Send(mm);
                    Console.WriteLine("Email sent");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public static DataStore SearchWebsite()
        {
            var html = @"http://readonepunchman.net//";
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);

            var node = htmlDoc.DocumentNode.SelectSingleNode("//body");

            string fullName = "";
            string link = "";

            HtmlNode[] nodes = htmlDoc.DocumentNode.SelectNodes("//a").ToArray();
            foreach (HtmlNode item in nodes)
            {
                if (item.InnerText.Contains("Onepunch-Man – Chapter "))
                {
                    fullName = item.InnerText.ToString().Replace("\n", "");
                    int linkFrom = item.OuterHtml.IndexOf(@"""") + 1;
                    int linkTo = item.OuterHtml.LastIndexOf(@"""");
                    link = item.OuterHtml.Substring(linkFrom, linkTo - linkFrom).Trim();

                    break;
                }
            }

            int nameFrom = fullName.Trim().LastIndexOf(" ") + 1;
            int nameTo = fullName.Trim().Length;

            DataStore dataList = new DataStore();
            dataList.FullName = fullName.Trim();
            dataList.Issue = Convert.ToDecimal(fullName.Substring(nameFrom, nameTo - nameFrom));
            dataList.Link = link;
            dataList.DateFound = DateTime.Today;

            return dataList;
        }
    }
}
