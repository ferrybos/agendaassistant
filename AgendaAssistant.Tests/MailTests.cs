using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Vluchtprikker.DB;
using Vluchtprikker.DB.Repositories;
//using Vluchtprikker.Entities;
using Vluchtprikker.Services;
using Vluchtprikker.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Vluchtprikker.Tests
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
            _mailService = new MailService(new VluchtprikkerDbContext());

            _event = new Event()
                {
                    ID = Guid.NewGuid(),
                    Title = "Weekendje Valencia",
                    OrganizerName = "Ferry Bos",
                    OrganizerEmail = "ferrybos@gmail.com"
                };

            _participant = new Participant() { ID = Guid.NewGuid(), EventID = _event.ID, Email = "ferrybos@gmail.com", Name = "Ferry" };
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
            reminderBuilder.AppendLine(_event.OrganizerName);

            _mailService.SendMessage(_event, _participant, reminderBuilder.ToString());
        }
    }
}
