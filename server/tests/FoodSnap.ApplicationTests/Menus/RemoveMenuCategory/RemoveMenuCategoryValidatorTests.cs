using System;
using System.Threading.Tasks;
using FoodSnap.Application.Menus.RemoveMenuCategory;
using Xunit;

namespace FoodSnap.ApplicationTests.Menus.RemoveMenuCategory
{
    public class RemoveMenuCategoryValidatorTests
    {
        private readonly RemoveMenuCategoryValidator validator;

        public RemoveMenuCategoryValidatorTests()
        {
            validator = new RemoveMenuCategoryValidator();
        }

        [Fact]
        public async Task RestaurantId_Cant_Be_Empty()
        {
            var command = new RemoveMenuCategoryCommand
            {
                RestaurantId = Guid.Empty,
                CategoryName = "Pizza",
            };

            var result = await validator.Validate(command);

            Assert.False(result.IsSuccess);
            Assert.True(result.Error.Errors.ContainsKey(nameof(command.RestaurantId)));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task CategoryName_Cant_Be_Empty(string name)
        {
            var command = new RemoveMenuCategoryCommand
            {
                RestaurantId = Guid.NewGuid(),
                CategoryName = name,
            };

            var result = await validator.Validate(command);

            Assert.False(result.IsSuccess);
            Assert.True(result.Error.Errors.ContainsKey(nameof(command.CategoryName)));
        }
    }
}
