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
    
    public partial class Flight
    {
        public Flight()
        {
            this.Availabilities = new HashSet<Availability>();
            this.FlightSearches = new HashSet<FlightSearch>();
        }
    
        public System.DateTime DepartureDate { get; set; }
        public System.DateTime STD { get; set; }
        public System.DateTime STA { get; set; }
        public string CarrierCode { get; set; }
        public int FlightNumber { get; set; }
        public long FlightSearchID { get; set; }
        public int Price { get; set; }
        public long ID { get; set; }
        public bool Enabled { get; set; }
    
        public virtual ICollection<Availability> Availabilities { get; set; }
        public virtual FlightSearch FlightSearch { get; set; }
        public virtual ICollection<FlightSearch> FlightSearches { internal get; set; }
    }
}
