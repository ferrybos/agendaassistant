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

                var dbEvent = db.Events.Create();
                dbEvent.CreatedUtc = DateTime.UtcNow;
                dbEvent.Title = value.Title;
                dbEvent.Description = value.Description;
                dbEvent.Status = "";
                dbEvent.IsConfirmed = false;
                dbEvent.Person = organizerPerson;
                db.Events.Add(dbEvent);

                db.SaveChanges();
            }

            return null;
        }
    }
}
