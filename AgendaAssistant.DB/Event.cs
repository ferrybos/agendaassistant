//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AgendaAssistant.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class Event
    {
        public Event()
        {
            this.Emails = new HashSet<Email>();
            this.Participants = new HashSet<Participant>();
        }
    
        public System.Guid ID { get; set; }
        public System.DateTime CreatedUtc { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsConfirmed { get; set; }
        public Nullable<long> OutboundFlightSearchID { get; set; }
        public Nullable<long> InboundFlightSearchID { get; set; }
        public System.Guid OrganizerPersonID { get; set; }
        public Nullable<short> StatusID { get; set; }
    
        public virtual ICollection<Email> Emails { get; set; }
        public virtual FlightSearch InboundFlightSearch { get; set; }
        public virtual Person Organizer { get; set; }
        public virtual FlightSearch OutboundFlightSearch { get; set; }
        public virtual EventStatus EventStatus { get; set; }
        public virtual ICollection<Participant> Participants { get; set; }
    }
}
