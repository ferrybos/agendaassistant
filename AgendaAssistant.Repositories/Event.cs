//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AgendaAssistant.Repositories
{
    using System;
    using System.Collections.Generic;
    
    public partial class Event
    {
        public Event()
        {
            this.Comments = new HashSet<Comment>();
            this.People = new HashSet<Person>();
        }
    
        public long ID { get; set; }
        public System.DateTime CreatedUtc { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long OrganizerPersonID { get; set; }
        public string Status { get; set; }
        public bool IsConfirmed { get; set; }
        public Nullable<long> OutboundFlightSearchID { get; set; }
        public Nullable<long> InboundFlightSearchID { get; set; }
    
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual FlightSearch FlightSearch { get; set; }
        public virtual Person Person { get; set; }
        public virtual FlightSearch FlightSearch1 { get; set; }
        public virtual ICollection<Person> People { get; set; }
    }
}