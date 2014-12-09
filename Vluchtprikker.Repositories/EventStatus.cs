using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vluchtprikker.Repositories
{
    public static class EventStatusEnum
    {
        public const short New = 0;
        public const short ParticipantsAdded = 1;
        public const short NewCompleted = 9;
        public const short Confirmed = 10;
        public const short InvitationsSent = 11;
        public const short AvailabilityCompleted = 20; // intermediate status
        public const short BookingDetailsCompleted = 25;  // intermediate status
        public const short PushpinActive = 50; // allowed if status >= AvailabilityCompleted
        public const short PushpinCompleted = 55; // allowed if status >= PushpinActive
        public const short BookingActive = 80; // allowed if status >= BookingDetailsCompleted
        public const short BookingCompleted = 85; // allowed if status >= BookingActive (enter PNR)
        public const short Closed = 99;
    }
}
