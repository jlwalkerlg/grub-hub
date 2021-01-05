using System;
using System.Threading.Tasks;
using Web.Features.Users.UpdateAuthUserDetails;
using Web.Domain;
using Web.Domain.Users;
using Xunit;

namespace WebTests.Actions.Users.UpdateAuthUserDetails
{
    public class UpdateAuthUserDetailsActionTests : WebActionTestBase
    {
        public UpdateAuthUserDetailsActionTests(WebActionTestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task It_Requires_Authentication()
        {
            var response = await Put("/auth/user", new UpdateAuthUserDetailsCommand
            {
                Name = "Bruno",
                Email = "bruno@gmail.com",
            });

            Assert.Equal(401, (int)response.StatusCode);
        }

        [Fact]
        public async Task It_Returns_Validation_Errors()
        {
            var user = new RestaurantManager(
                new UserId(Guid.NewGuid()),
                "Jordan Walker",
                new Email("walker.jlg@gmail.com"),
                "password123");

            await Login(user);

            var response = await Put("/auth/user", new UpdateAuthUserDetailsCommand
            {
                Name = "",
                Email = "",
            });

            var errors = await response.GetErrors();
            Assert.True(errors.ContainsKey("name"));
            Assert.True(errors.ContainsKey("email"));
        }
    }
}
