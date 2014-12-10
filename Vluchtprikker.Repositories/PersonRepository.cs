﻿//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vluchtprikker.DB.Repositories;
//using Vluchtprikker.Entities;
//using Vluchtprikker.Extensions;
//using Vluchtprikker.Shared;
//using Person = Vluchtprikker.DB.Person;

//namespace Vluchtprikker.Repositories
//{
//    /// <summary>
//    /// Contains all logic to interface with data(base)
//    /// </summary>
//    public class PersonRepository : DbRepository
//    {
//        public PersonRepository(IDbContext dbContext)
//            : base(dbContext)
//        {
//        }

//        public Person Single(Guid id)
//        {
//            var dbPerson = DbContext.Persons.Single(e => e.ID.Equals(id));

//            if (dbPerson == null)
//            {
//                throw new ApplicationException(string.Format("Person not found with id {0}", id));
//            }

//            return dbPerson;
//        }

//        public Person AddOrGetExisting(string name, string email)
//        {
//            var dbPerson = SingleOrDefault(email);

//            if (dbPerson == null)
//            {
//                return Add(name, email);
//            }

//            if (!name.Equals(dbPerson.Name, StringComparison.InvariantCultureIgnoreCase))
//                throw new FormattedException("Person with email {0} already exists under a different name", email);

//            return dbPerson;
//        }

//        public void Update(Person dbPerson, string firstNameInPassport, string lastNameInPassport, DateTime? dateOfBirth, Gender? gender)
//        {
//            dbPerson.FirstNameInPassport = firstNameInPassport;
//            dbPerson.LastNameInPassport = lastNameInPassport;
//            dbPerson.DateOfBirth = dateOfBirth;
//            dbPerson.Gender = gender.HasValue ? (byte)gender : (byte?)null;
//        }

//        /// <summary>
//        /// Fetches an event from the database with the given id 
//        /// </summary>
//        private Person SingleOrDefault(string email)
//        {
//            return DbContext.Persons.SingleOrDefault(e => e.Email.Equals(email));
//        }

//        private Person Add(string name, string email)
//        {
//            if (string.IsNullOrWhiteSpace(name))
//                throw new FormattedException("Cannot add person. Please specify a name");

//            if (string.IsNullOrWhiteSpace(email))
//                throw new FormattedException("Cannot add person. Please specify an email");

//            var dbPerson = DbContext.Persons.Create();
//            DbContext.Persons.Add(dbPerson);

//            dbPerson.ID = Guid.NewGuid();
//            dbPerson.Name = name;
//            dbPerson.Email = email;

//            DbContext.SaveChanges();

//            return dbPerson;
//        }

//    }
//}