using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.DB;
using AgendaAssistant.DB.Repositories;
using AgendaAssistant.Entities;
using AgendaAssistant.Extensions;
using AutoMapper;
using Event = AgendaAssistant.Entities.Event;
using Person = AgendaAssistant.Entities.Person;

namespace AgendaAssistant.Repositories
{
    public interface IEventRepository
    {
        Event Save(Event value);
        Event Get(string code);
        void Confirm(string code);
    }

    /// <summary>
    /// Contains all logic to interface with data(base)
    /// </summary>
    public class EventRepository : IEventRepository
    {
        private AgendaAssistantEntities _db;

        public EventRepository()
        {
            _db = DbContextFactory.New();
        }

        public Event Save(Event value)
        {
            var dbPersonRepository = new DbPersonRepository(_db);
            var dbEventRepository = new DbEventRepository(_db);

            var organizerPerson = dbPersonRepository.AddPerson(value.Organizer.Name, value.Organizer.Email);
            var dbEvent = dbEventRepository.AddEvent(value.Title, value.Description, organizerPerson, "Niet bevestigd", false);

            // add participants
            foreach (var participant in value.Participants)
            {
                var person = value.Organizer.Matches(participant.Name, participant.Email)
                                    ? organizerPerson
                                    : dbPersonRepository.AddPerson(participant.Name, participant.Email);

                dbEventRepository.AddParticipant(dbEvent, person);
            }

            _db.SaveChanges();

            return EntityMapper.Map(dbEvent);
        }

        public void Confirm(string code)
        {
            var dbEvent = GetDbEvent(code);

            if (!dbEvent.IsConfirmed)
            {
                dbEvent.IsConfirmed = true;
                dbEvent.Status = "Uitnodigingen verstuurd";
                _db.SaveChanges();
            }
        }

        public Event Get(string code)
        {
            var dbEvent = GetDbEvent(code);

            return EntityMapper.Map(dbEvent);
        }

        private DB.Event GetDbEvent(string code)
        {
            return new DbEventRepository(_db).Get(CodeString.CodeStringToGuid(code));
        }
    }
}
