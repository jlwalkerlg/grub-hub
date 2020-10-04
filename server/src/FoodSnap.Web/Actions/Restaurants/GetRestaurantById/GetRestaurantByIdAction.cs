using System;
using System.Threading.Tasks;
using FoodSnap.Web.Envelopes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FoodSnap.Domain;

namespace FoodSnap.Web.Actions.Restaurants.GetRestaurantById
{
    public class GetRestaurantByIdAction : Action
    {
        private readonly IMediator mediator;

        public GetRestaurantByIdAction(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("/restaurants/{id}")]
        public async Task<IActionResult> Execute([FromRoute] Guid id)
        {
            var query = new GetRestaurantByIdQuery
            {
                Id = id
            };

            var result = await mediator.Send(query);

            if (!result.IsSuccess)
            {
                return PresentError(result.Error);
            }

            if (result.Value == null)
            {
                return PresentError(Error.NotFound("Restaurant not found."));
            }

            return Ok(new DataEnvelope
            {
                Data = result.Value
            });
        }
    }
}
