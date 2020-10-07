using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FoodSnap.Application.Services.Authentication;
using FoodSnap.Domain;
using FoodSnap.Domain.Menus;
using FoodSnap.Shared;

namespace FoodSnap.Application.Menus.UpdateMenuItem
{
    public class UpdateMenuItemHandler : IRequestHandler<UpdateMenuItemCommand>
    {
        private readonly IAuthenticator authenticator;
        private readonly IUnitOfWork unitOfWork;

        public UpdateMenuItemHandler(IAuthenticator authenticator, IUnitOfWork unitOfWork)
        {
            this.authenticator = authenticator;
            this.unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateMenuItemCommand command, CancellationToken cancellationToken)
        {
            var menu = await unitOfWork.Menus.GetById(new MenuId(command.MenuId));

            if (menu == null)
            {
                return Result.Fail(Error.NotFound("Menu not found."));
            }

            var restaurant = await unitOfWork.Restaurants.GetById(menu.RestaurantId);

            if (restaurant.ManagerId != authenticator.UserId)
            {
                return Result.Fail(Error.Unauthorised("Only the restaurant owner can update the menu."));
            }

            var category = menu.Categories.FirstOrDefault(x => x.Name == command.CategoryName);

            if (category == null)
            {
                return Result.Fail(Error.NotFound($"Category {command.CategoryName} not found."));
            }

            var item = category.Items.FirstOrDefault(x => x.Name == command.OldItemName);

            if (item == null)
            {
                return Result.Fail(Error.NotFound($"Item {command.OldItemName} not found for category {command.CategoryName}."));
            }

            if (command.OldItemName != command.NewItemName && category.Items.Any(x => x.Name == command.NewItemName))
            {
                return Result.Fail(Error.BadRequest($"Item {command.NewItemName} already exists for category {command.CategoryName}."));
            }

            category.RenameItem(command.OldItemName, command.NewItemName);
            item.Description = command.Description;
            item.Price = new Money(command.Price);

            await unitOfWork.Commit();

            return Result.Ok();
        }
    }
}
