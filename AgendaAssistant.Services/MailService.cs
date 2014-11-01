using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.Entities;
using AgendaAssistant.Mail;

namespace AgendaAssistant.Services
{
    public class EmailBody
    {
        public string Text { get; set; }
        public string Html { get; set; }
    }

    public class MailService: IMailService
    {
        private const string From = "service@vluchtprikker.nl";
        private const string RootUrl = @"http://vluchtprikker.nl/#/";

        private string ConfirmEventUrl(Event evn)
        {
            return RootUrl + string.Format("confirmevent/{0}", evn.Code);
        }

        private string EventUrl(Event evn)
        {
            return RootUrl + string.Format("event/{0}", evn.Code);
        }

        private string AvailabilityUrl(Event evn, Participant participant)
        {
            return RootUrl + string.Format("availability/{0}/{1}", evn.Code, participant.PersonId);
        }

        public void Send(string recipient, string subject, EmailBody body)
        {
            Send(new List<string>() {recipient}, subject, body);
        }

        public void Send(List<String> recipients, string subject, EmailBody body)
        {
            var mailer = new SendGridMail();
            var msg = mailer.CreateMessage(From, recipients, subject, body.Html, body.Text);
            mailer.SendMessage(msg);
        }

        public void SendToOrganizer(Event evn, string subject, string announcement, string action, string linkUrl, string linkText)
        {
            var salutation = string.Format("Beste {0}", evn.Organizer.Name);
            var body = GenerateBody(salutation, announcement, action, linkUrl, linkText);

            Send(evn.Organizer.Email, subject, body);
        }

        public void SendToParticipant(Participant participant, string subject, string announcement, string action, string linkUrl, string linkText)
        {
            var salutation = string.Format("Beste {0}", participant.Name);
            var body = GenerateBody(salutation, announcement, action, linkUrl, linkText);
            
            Send(participant.Email, subject, body);
        }

        public void SendEventConfirmation(Event evn)
        {
            SendToOrganizer(
                evn, 
                string.Format("Uitnodigingen versturen: {0}", evn.Title),
                string.Format("U heeft de afspraak '<strong>{0}</strong>' aangemaakt.", evn.Title), 
                "Klik op de onderstaande link om de uitnodigingen te versturen.",
                EventUrl(evn), 
                "Uitnodigingen versturen");
        }

        public void SendInvitation(Event evn, Participant participant)
        {
            SendToParticipant(
                participant,
                string.Format("Uitnodiging van {0}: {1}", evn.Organizer.Name, evn.Title),
                string.Format("{0} wil een vlucht prikken voor de afspraak '{1}'.", evn.Organizer.Name, evn.Title),
                "Klik op de onderstaande link om de afspraak te bekijken en uw beschikbaarheid op te geven.",
                AvailabilityUrl(evn, participant),
                "Beschikbaarheid invullen"
                );
        }

        public void SendMessage(Event evn, Participant participant, string text)
        {
            SendToParticipant(
                participant,
                string.Format("Extra bericht: {0}", evn.Title),
                text,
                "",
                EventUrl(evn),
                "Afspraak bekijken"
                );
        }

        public void SendInvitationConfirmation(Event evn)
        {
            SendToOrganizer(
                evn,
                string.Format("Afspraak beheren: {0}", evn.Title),
                string.Format("Uw uitnodigingen voor de afspraak '<strong>{0}</strong>' zijn verstuur.", evn.Title),
                "Klik op de onderstaande link om de reacties te bekijken, de afspraak te wijzigen of af te ronden.",
                EventUrl(evn),
                "Afspraak beheren");
        }

        private EmailBody GenerateBody(string salutation, string announcement, string action, string linkUrl, string linkText)
        {
            const string logoUrl = @"http://www.vluchtprikker.nl/img/logo.png\";

            var htmlBuilder = new StringBuilder();
            htmlBuilder.AppendLine(salutation);
            htmlBuilder.AppendLine("<br /><br />");
            htmlBuilder.AppendLine(announcement);
            htmlBuilder.AppendLine(action);
            htmlBuilder.AppendLine("<br /><br />");
            htmlBuilder.AppendLine(string.Format("<a href=\"{0}\" style=\"Font-size:22px;line-height:32px;color:#3399ff;\">{1}</a>", linkUrl, linkText));
            htmlBuilder.AppendLine("<br /><br /><br />");
            htmlBuilder.AppendLine(string.Format("<img src=\"{0}\" style=\"display:block;\" /><br />", logoUrl));
            htmlBuilder.AppendLine(string.Format(
                "<span style=\"font-size:11px;color:#777777;\">Werkt de link niet? Kopieer dan het onderstaande adres naar uw browser.<br />{0}</span>", linkUrl));
            htmlBuilder.AppendLine("");

            var textBuilder = new StringBuilder();
            textBuilder.AppendLine(salutation);
            textBuilder.AppendLine("");
            textBuilder.AppendLine(announcement);
            textBuilder.AppendLine(action);
            textBuilder.AppendLine("");
            textBuilder.AppendLine(linkUrl);

            return new EmailBody() {Html = htmlBuilder.ToString(), Text = textBuilder.ToString()};
        }
    }

    public interface IMailService
    {
        void Send(List<String> recipients, string subject, EmailBody body);
        void Send(string recipient, string subject, EmailBody body);

        void SendEventConfirmation(Event evn);
        void SendInvitation(Event evn, Participant participant);
        void SendInvitationConfirmation(Event evn);
        void SendMessage(Event evn, Participant participant, string text);

        void SendToOrganizer(Event evn, string subject, string announcement, string action, string linkUrl,
                             string linkText);

        void SendToParticipant(Participant participant, string subject, string announcement, string action,
                               string linkUrl, string linkText);
    }
}
