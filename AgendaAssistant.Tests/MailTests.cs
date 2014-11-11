using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using AgendaAssistant.DB;
using AgendaAssistant.DB.Repositories;
//using AgendaAssistant.Entities;
using AgendaAssistant.Services;
using AgendaAssistant.Shared;
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
            _mailService = new MailService(new AgendaAssistantDbContext());

            _event = new Event()
                {
                    ID = Guid.NewGuid(),
                    Title = "Weekendje Valencia",
                    Organizer = new DB.Person() {Email = "ferrybos@gmail.com", Name = "Ferry Bos"}
                };

            var person = new Person() {ID = Guid.NewGuid(), Email = "ferrybos@gmail.com", Name = "Ferry"};
            _participant = new Participant() {ID = Guid.NewGuid(), EventID = _event.ID, Person = person};
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
