using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.Entities;
using AgendaAssistant.Repositories;

namespace AgendaAssistant.Services
{
    public interface IParticipantService
    {
        Participant Get(string eventCode, long personId);
        void Update(Participant participant);
    }

    public class ParticipantService : IParticipantService
    {
        private readonly IParticipantRepository _repository;

        public ParticipantService(IParticipantRepository repository)
        {
            _repository = repository;
        }

        public Participant Get(string eventCode, long personId)
        {
            return _repository.Get(eventCode, personId);
        }

        public void Update(Participant participant)
        {
            _repository.Update(participant);
        }
    }
}
