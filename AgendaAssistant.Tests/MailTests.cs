using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using AgendaAssistant.Entities;
using AgendaAssistant.Mail;
using AgendaAssistant.Repositories;
using AgendaAssistant.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgendaAssistant.Tests
{
    [TestClass]
    public class MailTests
    {
        private MailService _mailService;
        private Event _event;
        private Participant _participant;

        [TestInitialize]
        public void Setup()
        {
            _mailService = new MailService();

            _event = new Event()
                {
                    Id = Guid.NewGuid(),
                    Title = "Weekendje Valencia",
                    Organizer = new Person() {Email = "ferrybos@gmail.com", Name = "Ferry Bos"}
                };

            var person = new Person() {Id = Guid.NewGuid(), Email = "ferrybos@gmail.com", Name = "Ferry"};
            _participant = new Participant() {Id = Guid.NewGuid(), EventId = _event.Id, Person = person};
        }

        [TestMethod]
        public void TestMail()
        {
            var body = new EmailBody() {Html = "<p>Dit is een test!</p>", Text = "Dit is een test!"};
            _mailService.Send("ferrybos@gmail.com", "Test from SendGrid", body);
        }

        [TestMethod]
        public void EventConfirmation()
        {
            _mailService.SendEventConfirmation(_event);
        }

        [TestMethod]
        public void Invitation()
        {
            _mailService.SendInvitation(_event, _participant);
        }

        [TestMethod]
        public void ConfirmInvitationsSent()
        {
            _mailService.SendInvitationConfirmation(_event);
        }

        [TestMethod]
        public void Reminder()
        {
            var reminderBuilder = new StringBuilder();
            reminderBuilder.AppendLine(
                "Onlangs heb ik u uitgenodigd voor de afspraak 'Weekendje Valencia?' om een datum te kunnen prikken. Helaas heb ik nog geen reactie van u mogen ontvangen.");
            reminderBuilder.AppendLine(""); 
            reminderBuilder.AppendLine(
                "Graag hoor ik van u op welke data u beschikbaar bent. U kunt de afspraak bekijken en uw beschikbaarheid opgeven door op de onderstaande link te klikken.");
            reminderBuilder.AppendLine("");
            reminderBuilder.AppendLine("Met vriendelijke groet,");
            reminderBuilder.AppendLine(""); 
            reminderBuilder.AppendLine(_event.Organizer.Name);

            _mailService.SendMessage(_event, _participant, reminderBuilder.ToString());
        }
    }
}
