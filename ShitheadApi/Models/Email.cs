using System.Collections.Generic;

namespace ShitheadApi.Models
{
    public class Email
    {
        public string Adress { get; set; }

        public string Title { get; set; }

        public string Subject { get; set; }

        public string Name { get; set; }

        public string Html { get; set; }

        public List<Attachment> Attachments { get; set; }

        public List<EmailAddress> Bccs { get; set; }

        public List<EmailAddress> Ccs { get; set; }

        public Email(string adress,
                     string title,
                     string subject,
                     string name,
                     string html,
                     List<Attachment> attachments = null,
                     List<EmailAddress> bccs = null,
                     List<EmailAddress> ccs = null)
        {
            Adress = adress;
            Title = title;
            Subject = subject;
            Name = name;
            Html = html;
            Attachments = attachments;
            Bccs = bccs;
            Ccs = ccs;
        }
    }
}
