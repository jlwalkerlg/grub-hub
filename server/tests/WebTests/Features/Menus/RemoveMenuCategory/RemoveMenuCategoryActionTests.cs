using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace WebTests.Features.Menus.RemoveMenuCategory
{
    public class RemoveMenuCategoryActionTests : ActionTestBase
    {
        public RemoveMenuCategoryActionTests(ActionTestWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task It_Requires_Authentication()
        {
            var response = await GetClient().Delete(
                $"/restaurants/{Guid.NewGuid()}/menu/categories/{Guid.NewGuid()}");

            response.StatusCode.ShouldBe(401);
        }

        [Fact]
        public async Task It_Returns_Handler_Errors()
        {
            var response = await GetAuthenticatedClient().Delete(
                $"/restaurants/{Guid.NewGuid()}/menu/categories/{Guid.NewGuid()}");

            response.StatusCode.ShouldBe(400);
        }
    }
}
