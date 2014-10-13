using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.DB;
using AgendaAssistant.Entities;
using Event = AgendaAssistant.Entities.Event;
using Person = AgendaAssistant.Entities.Person;

namespace AgendaAssistant.Repositories
{
    public interface IEventRepository
    {
        Event CreateNew(Event value);
        Event Get(long id);
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

        public Event Get(long id)
        {
            using (var db = new AgendaAssistantEntities())
            {
                var result = db.Events.SingleOrDefault(e => e.ID == id);

                if (result == null)
                {
                    throw new ApplicationException(string.Format("Event not found with ID {0}", id));
                }

                // todo: AutoMapper
                return new Event
                    {
                        EventId = result.ID, Title = result.Title, Description = result.Description, Status = result.Status, IsConfirmed = result.IsConfirmed,
                        Organizer = new Person() { PersonId = result.Person.ID, Name = result.Person.Name, Email = result.Person.Email}
                    };
            }
        }
    }
}
