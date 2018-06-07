using System;
using NServiceBus;

namespace MobileGameCreditsSaga
{
    public class OrderItemSucceeded : IEvent
    {
        public Guid UserId { get; set; }

        public Guid ItemId { get; set; }

        public int NewCredit { get; set; }
    }
}