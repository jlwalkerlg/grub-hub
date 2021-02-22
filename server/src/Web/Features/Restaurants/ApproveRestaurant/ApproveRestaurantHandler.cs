﻿using System.Threading;
using System.Threading.Tasks;
using Web.Domain.Restaurants;
using Web.Services.Clocks;

namespace Web.Features.Restaurants.ApproveRestaurant
{
    public class ApproveRestaurantHandler : IRequestHandler<ApproveRestaurantCommand>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IClock clock;

        public ApproveRestaurantHandler(IUnitOfWork unitOfWork, IClock clock)
        {
            this.unitOfWork = unitOfWork;
            this.clock = clock;
        }

        public async Task<Result> Handle(
            ApproveRestaurantCommand command,
            CancellationToken cancellationToken)
        {
            var id = new RestaurantId(command.RestaurantId);
            var restaurant = await unitOfWork.Restaurants.GetById(id);

            if (restaurant is null)
            {
                return Error.NotFound("Restaurant not found.");
            }

            restaurant.Approve();

            var @event = new RestaurantApprovedEvent(restaurant.Id, clock.UtcNow);
            await unitOfWork.Events.Add(@event);

            await unitOfWork.Commit();

            return Result.Ok();
        }
    }
}
