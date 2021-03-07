﻿using System.Threading;
using System.Threading.Tasks;
using Stripe;

namespace Web.Features.Orders
{
    public class RefundOrderProcessor : JobProcessor<RefundOrderJob>
    {
        private readonly PaymentIntentService service = new();

        public async Task<Result> Handle(RefundOrderJob job, CancellationToken cancellationToken)
        {
            await service.CancelAsync(
                job.PaymentIntentId,
                new PaymentIntentCancelOptions(),
                cancellationToken: cancellationToken);

            return Result.Ok();
        }
    }
}
