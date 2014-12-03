using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public interface IEventRepository
    {
        Event Get(Guid id);
        void Save();
    }

    public class Event
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public Person Organizer { get; set; }

        public bool IsConfirmed { get; set; }
    }
}
