using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Vluchtprikker.DB;
using Vluchtprikker.Mail;
using Vluchtprikker.Shared;

namespace Vluchtprikker.Services
{
    public class EmailBody
    {
        public string Text { get; set; }
        public string Html { get; set; }
    }

    public class MailService: IMailService
    {
        private readonly string _from = "service@vluchtprikker.nl";
        private readonly string _rootUrl = @"http://vluchtprikker.nl";

        public MailService()
        {
            if (HttpContext.Current != null)
            {
                _rootUrl = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "");
            }
        }

        private string ConfirmEventUrl(Event dbEvent)
        {
            return _rootUrl + string.Format("/#/confirm/{0}", GuidUtil.ToString(dbEvent.ID));
        }
        
        private string EventUrl(Event dbEvent)
        {
            return _rootUrl + string.Format("/#/event/{0}", GuidUtil.ToString(dbEvent.ID));
        }

        private string AvailabilityUrl(Participant dbParticipant)
        {
            return _rootUrl + string.Format("/#/availability/{0}", GuidUtil.ToString(dbParticipant.ID));
        }

        public void Send(string recipient, string subject, EmailBody body)
        {
            Send(new List<string>() {recipient}, subject, body);
        }

        public void SendBookingCreatedEmail(Event dbEvent, Participant dbParticipant)
        {
            var subject = string.Format("{0}: Vluchten zijn geboekt!", dbEvent.Title);

            var salutation = string.Format("Beste {0}", dbParticipant.Name);
            var announcement = string.Format("Hieronder vind u de definitieve vlucht informatie voor de afspraak '{0}'.", dbEvent.Title);

            var htmlBuilder = new StringBuilder();
            htmlBuilder.AppendLine(salutation);
            htmlBuilder.AppendLine("<br /><br />");
            htmlBuilder.AppendLine(announcement);
            htmlBuilder.AppendLine("<br /><br />");

            var textBuilder = new StringBuilder();
            textBuilder.AppendLine(salutation);
            textBuilder.AppendLine("");
            textBuilder.AppendLine(announcement);
            textBuilder.AppendLine("");

            AddFlightInformation(dbEvent, htmlBuilder, textBuilder);

            var body = new EmailBody() { Html = htmlBuilder.ToString(), Text = textBuilder.ToString() };

            Send(dbParticipant.Email, subject, body);
        }

        private void AddFlightInformation(Event dbEvent, StringBuilder htmlBuilder, StringBuilder textBuilder)
        {
            var selectedOutboundFlight = dbEvent.OutboundFlightSearch.SelectedFlight;
            var selectedInboundFlight = dbEvent.InboundFlightSearch.SelectedFlight;

            htmlBuilder.AppendLine(string.Format("Transavia boekingsnummer: <strong>{0}</strong><br /><br />", dbEvent.PNR));

            htmlBuilder.AppendLine(string.Format("<strong>Heen vlucht: {0}</strong>",
                                                 selectedOutboundFlight.DepartureDate.ToString("dddd d MMMM",
                                                                                               CultureInfo.GetCultureInfo(
                                                                                                   "nl-NL"))));
            htmlBuilder.AppendLine("<br />Vertrek: " + selectedOutboundFlight.STD.ToString("HH:mm"));
            htmlBuilder.AppendLine("<br />Aankomst: " + selectedOutboundFlight.STA.ToString("HH:mm"));
            htmlBuilder.AppendLine("<br />Vluchtnummer: " + selectedOutboundFlight.CarrierCode + " " +
                                   selectedOutboundFlight.FlightNumber.ToString());
            htmlBuilder.AppendLine("<br /><br />");
            htmlBuilder.AppendLine(string.Format("<strong>Terug vlucht: {0}</strong>",
                                                 selectedInboundFlight.DepartureDate.ToString("dddd d MMMM",
                                                                                              CultureInfo.GetCultureInfo("nl-NL"))));
            htmlBuilder.AppendLine("<br />Vertrek: " + selectedInboundFlight.STD.ToString("HH:mm"));
            htmlBuilder.AppendLine("<br />Aankomst: " + selectedInboundFlight.STA.ToString("HH:mm"));
            htmlBuilder.AppendLine("<br />Vluchtnummer: " + selectedInboundFlight.CarrierCode + " " +
                                   selectedInboundFlight.FlightNumber.ToString());
            htmlBuilder.AppendLine("<br /><br />");

            textBuilder.AppendLine(string.Format("Transavia boekingsnummer: {0}", dbEvent.PNR));

            textBuilder.AppendLine(string.Format("Heen vlucht: {0}", selectedOutboundFlight.DepartureDate.ToString("dddd d MMMM", CultureInfo.GetCultureInfo("nl-NL"))));
            textBuilder.AppendLine("Vertrek: " + selectedOutboundFlight.STD.ToString("HH:mm"));
            textBuilder.AppendLine("Aankomst: " + selectedOutboundFlight.STA.ToString("HH:mm"));
            textBuilder.AppendLine("Vluchtnummer: " + selectedOutboundFlight.CarrierCode + " " + selectedOutboundFlight.FlightNumber.ToString());
            textBuilder.AppendLine("");
            textBuilder.AppendLine(string.Format("Terug vlucht: {0}", selectedInboundFlight.DepartureDate.ToString("dddd d MMMM", CultureInfo.GetCultureInfo("nl-NL"))));
            textBuilder.AppendLine("Vertrek: " + selectedInboundFlight.STD.ToString("HH:mm"));
            textBuilder.AppendLine("Aankomst: " + selectedInboundFlight.STA.ToString("HH:mm"));
            textBuilder.AppendLine("Vluchtnummer: " + selectedInboundFlight.CarrierCode + " " + selectedInboundFlight.FlightNumber.ToString());
            textBuilder.AppendLine("");
        }

        public void SendReminder(Event dbEvent, Participant dbParticipant)
        {
            if (dbParticipant.Email.Equals(dbEvent.OrganizerEmail))
                return;

            if (dbParticipant.AvailabilityConfirmed && dbParticipant.BookingDetailsConfirmed)
                return;

            string confirmedText = "";
            if (!dbParticipant.AvailabilityConfirmed && !dbParticipant.BookingDetailsConfirmed)
                confirmedText = "beschikbaarheid en boekingsgegevens";
            else if (!dbParticipant.AvailabilityConfirmed && dbParticipant.BookingDetailsConfirmed)
                confirmedText = "beschikbaarheid";
            else if (dbParticipant.AvailabilityConfirmed && !dbParticipant.BookingDetailsConfirmed)
                confirmedText = "boekingsgegevens";

            string announcement = string.Format("U heeft uw {0} nog niet bevestigd voor de afspraak '{1}'.",
                                                confirmedText, dbEvent.Title);

            SendToParticipant(
                dbParticipant,
                string.Format("{0}: Herinnering", dbEvent.Title),
                announcement,
                "Klik op de onderstaande link om de afspraak te bekijken en uw beschikbaarheid en boekingsgegevens op te geven.",
                AvailabilityUrl(dbParticipant),
                "Beschikbaarheid invullen of wijzigen"
                );
        }

        public void Send(List<String> recipients, string subject, EmailBody body)
        {
            var mailer = new SendGridMail();
            var msg = mailer.CreateMessage(_from, recipients, subject, body.Html, body.Text);
            mailer.SendMessage(msg);
        }

        public void SendToOrganizer(Event dbEvent, string subject, string announcement, string action, string linkUrl, string linkText)
        {
            var salutation = string.Format("Beste {0}", dbEvent.OrganizerName);
            var body = GenerateBody(salutation, announcement, action, linkUrl, linkText);

            Send(dbEvent.OrganizerEmail, subject, body);
        }

        public void SendToParticipant(Participant dbParticipant, string subject, string announcement, string action, string linkUrl, string linkText)
        {
            var salutation = string.Format("Beste {0}", dbParticipant.Name);
            var body = GenerateBody(salutation, announcement, action, linkUrl, linkText);

            Send(dbParticipant.Email, subject, body);
        }

        public void SendEventConfirmation(Event dbEvent)
        {
            SendToOrganizer(
                dbEvent, 
                string.Format("Uitnodigingen versturen: {0}", dbEvent.Title),
                string.Format("U heeft de afspraak '<strong>{0}</strong>' aangemaakt.", dbEvent.Title), 
                "Klik op de onderstaande link om de uitnodigingen te versturen. Deze link kunt u later ook gebruiken om de afspraak te beheren.",
                ConfirmEventUrl(dbEvent), 
                "Uitnodigingen versturen");
        }

        public void SendInvitation(Event dbEvent, Participant dbParticipant)
        {
            if (dbParticipant.Email.Equals(dbEvent.OrganizerEmail))
                return;

            string announcement = string.Format("{0} wil een vlucht prikken voor de afspraak '{1}'.",
                                                dbEvent.OrganizerName, dbEvent.Title);
           
            SendToParticipant(
                dbParticipant,
                string.Format("Uitnodiging van {0}: {1}", dbEvent.OrganizerName, dbEvent.Title),
                announcement,
                "Klik op de onderstaande link om de afspraak te bekijken en uw beschikbaarheid en boekingsgegevens op te geven. Bewaar deze email om later nog uw beschikbaarheid en boekingsgegevens te kunnen wijzigen.",
                AvailabilityUrl(dbParticipant),
                "Beschikbaarheid invullen of wijzigen"
                );
        }

        public void SendAvailabilityUpdate(Event dbEvent, Participant dbParticipant)
        {
            SendToOrganizer(
                        dbEvent,
                        string.Format("Beschikbaarheid van {0} voor {1}", dbParticipant.Name, dbEvent.Title),
                        string.Format("{0} heeft beschikbaarheid ingevuld voor de afspraak '<strong>{1}</strong>'.",
                                      dbParticipant.Name, dbEvent.Title),
                        "Klik op de onderstaande link om de beschikbaarheid te bekijken.",
                        EventUrl(dbEvent),
                        "Afspraak beheren");
        }

        public void SendMessage(Event dbEvent, Participant dbParticipant, string text)
        {
            SendToParticipant(
                dbParticipant,
                string.Format("Extra bericht: {0}", dbEvent.Title),
                text,
                "",
                EventUrl(dbEvent),
                "Afspraak bekijken"
                );
        }

        public void SendInvitationConfirmation(Event dbEvent)
        {
            SendToOrganizer(
                dbEvent,
                string.Format("Afspraak beheren: {0}", dbEvent.Title),
                string.Format("Uw uitnodigingen voor de afspraak '<strong>{0}</strong>' zijn verstuurd.", dbEvent.Title),
                "Klik op de onderstaande link om de reacties te bekijken, de afspraak te wijzigen of af te ronden.",
                EventUrl(dbEvent),
                "Afspraak beheren");
        }

        private EmailBody GenerateBody(string salutation, string announcement, string action, string linkUrl, string linkText)
        {
            //const string logoUrl = @"http://www.vluchtprikker.nl/img/logo.png\";

            var htmlBuilder = new StringBuilder();
            htmlBuilder.AppendLine(salutation);
            htmlBuilder.AppendLine("<br /><br />");
            htmlBuilder.AppendLine(announcement);
            htmlBuilder.AppendLine(action);
            htmlBuilder.AppendLine("<br /><br />");
            htmlBuilder.AppendLine(string.Format("<a href=\"{0}\" style=\"Font-size:22px;line-height:32px;color:#3399ff;\">{1}</a>", linkUrl, linkText));
            htmlBuilder.AppendLine("<br /><br /><br />");
            //htmlBuilder.AppendLine(string.Format("<img src=\"{0}\" style=\"display:block;\" /><br />", logoUrl));
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

        void SendBookingCreatedEmail(Event dbEvent, Participant dbParticipant);
        void SendEventConfirmation(Event dbEvent);
        void SendInvitation(Event dbEvent, Participant dbParticipant);
        void SendInvitationConfirmation(Event dbEvent);
        void SendAvailabilityUpdate(Event dbEvent, Participant dbParticipant);
        void SendMessage(Event dbEvent, Participant dbParticipant, string text);

        void SendReminder(Event dbEvent, Participant dbParticipant);

        void SendToOrganizer(Event dbEvent, string subject, string announcement, string action, string linkUrl,
                             string linkText);

        void SendToParticipant(Participant participant, string subject, string announcement, string action,
                               string linkUrl, string linkText);
    }
}
