using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.DB;
using Event = AgendaAssistant.Entities.Event;

namespace AgendaAssistant.Repositories
{
    public interface IEventRepository
    {
        Event CreateNew(Event value);
    }

    public class EventRepository : IEventRepository
    {
        public Event CreateNew(Event value)
        {
            using (var db = new AgendaAssistantEntities())
            {
                var organizerPerson = db.People.Create();
                organizerPerson.Name = value.Organizer.Name;
                organizerPerson.Email = value.Organizer.Email;
                db.People.Add(organizerPerson);

                // todo: create unique hashcode to use in email links
                var dbEvent = db.Events.Create();
                dbEvent.CreatedUtc = DateTime.UtcNow;
                dbEvent.Title = value.Title;
                dbEvent.Description = value.Description;
                dbEvent.Status = "";
                dbEvent.IsConfirmed = false;
                dbEvent.Person = organizerPerson;
                db.Events.Add(dbEvent);

                foreach (var participant in value.Participants)
                {
                    
                }

                db.SaveChanges();

                // todo: map dbEvent to Event
                value.EventId = dbEvent.ID;
            }

            return value;
        }
    }
}
