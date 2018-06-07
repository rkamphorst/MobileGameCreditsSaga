using System;
using NServiceBus;

namespace MobileGameCreditsSaga
{
    public class CustomerPreferredStatusChanged : IEvent
    {
        public Guid UserId { get; set; }
        public bool IsPreferred { get; set; }
    }
}