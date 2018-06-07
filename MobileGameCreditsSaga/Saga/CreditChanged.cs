using System;
using NServiceBus;

namespace MobileGameCreditsSaga
{
    public class CreditChanged : IEvent
    {
        public Guid UserId { get; set; }
        public int CreditChangeAmount { get; set; }
    }
}