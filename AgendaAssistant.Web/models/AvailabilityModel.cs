using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AgendaAssistant.Entities;

namespace AgendaAssistant.Web.models
{
    public class AvailabilityModel
    {
        public Event Event; // incl organizer, flightsearches
        public Person Person;
        public List<Availability> Availabilities;
    }
}