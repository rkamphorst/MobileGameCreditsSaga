using System;
using NServiceBus;

namespace MobileGameCreditsSaga
{
    public class OrderItem : ICommand
    {
        public Guid UserId { get; set; }
        public Guid ItemId { get; set; }
        public int Value { get; set; }
    }
}