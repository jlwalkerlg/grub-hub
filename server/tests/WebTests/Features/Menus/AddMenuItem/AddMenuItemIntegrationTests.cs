using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Web.Features.Menus.AddMenuItem;
using WebTests.TestData;
using Xunit;

namespace WebTests.Features.Menus.AddMenuItem
{
    public class AddMenuItemIntegrationTests : IntegrationTestBase
    {
        public AddMenuItemIntegrationTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task It_Adds_An_Item_To_The_Menu()
        {
            var manager = new User();

            var restaurant = new Restaurant()
            {
                ManagerId = manager.Id,
            };

            var category = new MenuCategory()
            {
                Name = "Pizza",
            };

            var menu = new Menu()
            {
                RestaurantId = restaurant.Id,
                Categories = new() { category }
            };

            fixture.Insert(manager, restaurant, menu);

            var request = new AddMenuItemRequest()
            {
                Name = "Margherita",
                Description = "Cheese & tomato",
                Price = 10m,
            };

            var response = await fixture.GetAuthenticatedClient(manager.Id).Post(
                $"/restaurants/{restaurant.Id}/menu/categories/{category.Id}/items",
                request);

            response.StatusCode.ShouldBe(201);

            var found = fixture.UseTestDbContext(db => db.MenuItems.Single());

            found.MenuCategoryId.ShouldBe(category.Id);
            found.Name.ShouldBe(request.Name);
            found.Description.ShouldBe(request.Description);
            found.Price.ShouldBe(request.Price);
        }
    }
}
