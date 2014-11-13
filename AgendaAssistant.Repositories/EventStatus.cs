using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaAssistant.Repositories
{
    public static class EventStatusEnum
    {
        public const short New = 0;
        public const short NewParticipantsCompleted = 1;
        public const short NewOutboundCompleted = 2;
        public const short NewInboundCompleted = 3;
        public const short NewCompleted = 9;
        public const short Confirmed = 10;
        public const short InvitationsSent = 11;
        public const short AvailabilityCompleted = 20;
        public const short BookingDetailsCompleted = 25;
        public const short PushpinActive = 50;
        public const short PushpinCompleted = 55;
        public const short BookingActive = 80;
        public const short BookingCompleted = 85;
        public const short Closed = 99;
    }
}
