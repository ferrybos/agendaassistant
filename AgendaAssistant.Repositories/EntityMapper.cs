using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.Entities;
using AgendaAssistant.Shared;

namespace AgendaAssistant.Repositories
{
    public static class EntityMapper
    {
        public static Event Map(DB.Event dbEvent)
        {
            return new Event()
            {
                EventId = dbEvent.ID,
                Code = CodeString.GuidAsCodeString(dbEvent.EventID),
                Description = dbEvent.Description,
                Status = dbEvent.Status,
                Title = dbEvent.Title,
                IsConfirmed = dbEvent.IsConfirmed,
                Organizer = Map(dbEvent.Organizer)
            };
        }

        public static Person Map(DB.Person person)
        {
            return new Person
                {
                    Id = person.ID,
                    Name = person.Name,
                    Email = person.Email
                };
        }
    }
}
