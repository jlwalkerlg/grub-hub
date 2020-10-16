using System;
using System.Threading.Tasks;
using FoodSnap.Application.Menus.RemoveMenuItem;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FoodSnap.Web.Actions.Menus.RemoveMenuItem
{
    public class RemoveMenuItemAction : Action
    {
        private readonly ISender sender;

        public RemoveMenuItemAction(ISender sender)
        {
            this.sender = sender;
        }

        [HttpDelete("/restaurants/{restaurantId}/menu/categories/{category}/items/{item}")]
        public async Task<IActionResult> Execute(
            [FromRoute] Guid restaurantId,
            [FromRoute] string category,
            [FromRoute] string item)
        {
            var command = new RemoveMenuItemCommand
            {
                RestaurantId = restaurantId,
                CategoryName = category,
                ItemName = item,
            };

            var result = await sender.Send(command);

            if (!result.IsSuccess)
            {
                return PresentError(result.Error);
            }

            return StatusCode(204);
        }
    }
}
