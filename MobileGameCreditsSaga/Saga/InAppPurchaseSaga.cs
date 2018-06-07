using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;

namespace MobileGameCreditsSaga
{
    internal class InAppPurchaseSaga : Saga<InAppPurchaseSaga.Model>,
        IAmStartedByMessages<OrderItem>,
        IAmStartedByMessages<CreditChanged>,
        IAmStartedByMessages<CustomerPreferredStatusChanged>
    {

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<Model> mapper)
        {
            mapper.ConfigureMapping<OrderItem>(m => m.UserId)
                .ToSaga(m => m.UserId);
            mapper.ConfigureMapping<CreditChanged>(m => m.UserId)
                .ToSaga(m => m.UserId);
            mapper.ConfigureMapping<CustomerPreferredStatusChanged>(m => m.UserId)
                .ToSaga(m => m.UserId);
        }
        

        public Task Handle(CreditChanged message, IMessageHandlerContext context)
        {
            Data.UserId = message.UserId;
            Data.Credit += message.CreditChangeAmount;
            return Task.CompletedTask;
        }

        public Task Handle(CustomerPreferredStatusChanged message, IMessageHandlerContext context)
        {
            Data.UserId = message.UserId;
            Data.IsPreferred = message.IsPreferred;
            return Task.CompletedTask;
        }

        public Task Handle(OrderItem message, IMessageHandlerContext context)
        {
            PruneOrders(TimeSpan.FromDays(14));

            var discount = Data.IsPreferred
                ? (GetTotalOrderValueFor(TimeSpan.FromDays(14)) > 100
                    ? (int) Math.Round(message.Value * .2m)
                    : (int) Math.Round(message.Value * .1m))
                : (GetTotalOrderValueFor(TimeSpan.FromDays(7)) > 100
                    ? (int) Math.Round(message.Value * .1m)
                    : 0);

            int amountDue = message.Value - discount;
            if (Data.Credit >= amountDue)
            {
                Data.Credit -= amountDue;
                Data.OrderAmounts.Add(new OrderAmount { OrderedAt = DateTime.UtcNow, Amount = message.Value });
                context.Publish(new OrderItemSucceeded
                {
                    UserId = message.UserId,
                    ItemId = message.ItemId,
                    NewCredit = Data.Credit
                });
            }
            else
            {
                context.Publish(new OrderItemSucceeded
                {
                    UserId = message.UserId,
                    ItemId = message.ItemId
                });
            }

            return Task.CompletedTask;
        }

        private void PruneOrders(TimeSpan keepTimespan)
        {
            Data.OrderAmounts = Data.OrderAmounts
                .Where(o => DateTime.UtcNow - o.OrderedAt < keepTimespan)
                .ToList();
        }

        private int GetTotalOrderValueFor(TimeSpan timespan)
        {
            return Data.OrderAmounts.Where(o => DateTime.UtcNow - o.OrderedAt < timespan).Sum(o => o.Amount);
        }

        internal class Model : ContainSagaData
        {
            public Guid UserId { get; set; }

            public bool IsPreferred { get; set; }

            public int Credit { get; set; }

            public List<OrderAmount> OrderAmounts { get; set; }

            
        }

        internal class OrderAmount
        {
            public DateTime OrderedAt { get; set; }

            public int Amount { get; set; }
        }
        
    }
}