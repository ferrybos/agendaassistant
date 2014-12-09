//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vluchtprikker.DB;
//using Vluchtprikker.DB.Repositories;
//using Vluchtprikker.Entities;
//using Participant = Vluchtprikker.Entities.Participant;
//using Person = Vluchtprikker.Entities.Person;

//namespace Vluchtprikker.Repositories
//{
//    public interface IEmailRepository
//    {
//        bool HasUnsent(int eventId, int recipientPersonId, short emailTypeId);

//        void Add(int eventId, int recipientPersonId, short emailTypeId, string subject, string bodyHtml,
//                  string bodyPlain);
//    }

//    public class EmailRepository : IEmailRepository
//    {
//        private readonly AgendaAssistantEntities _db;

//        public EmailRepository()
//        {
//            _db = DbContextFactory.New();
//        }

//        public bool HasUnsent(int eventId, int recipientPersonId, short emailTypeId)
//        {
//            //return new DbEmailRepository(_db).HasUnsent(eventId, recipientPersonId, emailTypeId);
//        }

//        public void Add(int eventId, int recipientPersonId, short emailTypeId, string subject, string bodyHtml,
//                  string bodyPlain)
//        {
//            //var dbEmail = new DbEmailRepository(_db).Add(eventId, recipientPersonId, emailTypeId, subject, bodyHtml, bodyPlain);

//            _db.SaveChanges();
//        }
//    }
//}
