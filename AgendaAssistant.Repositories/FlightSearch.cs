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
    
    public partial class FlightSearch
    {
        public FlightSearch()
        {
            this.Events = new HashSet<Event>();
            this.Events1 = new HashSet<Event>();
            this.Flights = new HashSet<Flight>();
        }
    
        public string DepartureStation { get; set; }
        public string ArrivalStation { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public long ID { get; set; }
    
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Event> Events1 { get; set; }
        public virtual ICollection<Flight> Flights { get; set; }
    }
}
