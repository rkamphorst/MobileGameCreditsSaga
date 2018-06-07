using System;
using NServiceBus;

namespace MobileGameCreditsSaga
{
    public class OrderItemFailed : IEvent
    {
        public Guid UserId { get; set; }

        public Guid ItemId { get; set; }

    }
}