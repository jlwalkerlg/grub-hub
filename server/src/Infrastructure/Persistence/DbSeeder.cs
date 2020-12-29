using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Restaurants.RegisterRestaurant;
using Application.Services.Hashing;
using Domain;
using Domain.Menus;
using Domain.Restaurants;
using Domain.Users;
using Infrastructure.Persistence.EF;

namespace Infrastructure.Persistence
{
    public class DbSeeder
    {
        private readonly AppDbContext context;
        private readonly IHasher hasher;

        public DbSeeder(AppDbContext context, IHasher hasher)
        {
            this.context = context;
            this.hasher = hasher;
        }

        public async Task Seed()
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "Infrastructure",
                "Persistence",
                "seed.json");
            var json = await File.ReadAllTextAsync(path);
            var jdoc = JsonDocument.Parse(json);

            foreach (var userEl in jdoc.RootElement.GetProperty("users").EnumerateArray())
            {
                var role = userEl.GetProperty("role").GetString();

                if (role == "RestaurantManager")
                {
                    var user = new RestaurantManager(
                        new UserId(new Guid(userEl.GetProperty("id").GetString())),
                        userEl.GetProperty("name").GetString(),
                        new Email(userEl.GetProperty("email").GetString()),
                        hasher.Hash("password123")
                    );

                    var restaurantEl = userEl.GetProperty("restaurant");

                    var restaurant = new Restaurant(
                        new RestaurantId(new Guid(restaurantEl.GetProperty("id").GetString())),
                        user.Id,
                        restaurantEl.GetProperty("name").GetString(),
                        new PhoneNumber(restaurantEl.GetProperty("phone_number").GetString()),
                        new Address(restaurantEl.GetProperty("address").GetString()),
                        new Coordinates(
                            (float)restaurantEl.GetProperty("latitude").GetDouble(),
                            (float)restaurantEl.GetProperty("longitude").GetDouble()
                        )
                    );

                    restaurant.MaxDeliveryDistanceInKm = restaurantEl.GetProperty("max_delivery_distance_in_km").GetInt32();
                    restaurant.EstimatedDeliveryTimeInMinutes =
                        restaurantEl.GetProperty("estimated_delivery_time_in_minutes").GetInt32();
                    restaurant.MinimumDeliverySpend = new Money(restaurantEl.GetProperty("minimum_delivery_spend").GetDecimal());
                    restaurant.DeliveryFee = new Money(restaurantEl.GetProperty("delivery_fee").GetDecimal());

                    if (restaurantEl.GetProperty("status").GetString() == "Approved")
                    {
                        restaurant.Approve();
                    }

                    restaurant.OpeningTimes = new OpeningTimes()
                    {
                        Monday = new OpeningHours(
                            TimeSpan.Parse(restaurantEl.GetProperty("monday_open").GetString()),
                            TimeSpan.Parse(restaurantEl.GetProperty("monday_close").GetString())
                        ),
                        Tuesday = new OpeningHours(
                            TimeSpan.Parse(restaurantEl.GetProperty("tuesday_open").GetString()),
                            TimeSpan.Parse(restaurantEl.GetProperty("tuesday_close").GetString())
                        ),
                        Wednesday = new OpeningHours(
                            TimeSpan.Parse(restaurantEl.GetProperty("wednesday_open").GetString()),
                            TimeSpan.Parse(restaurantEl.GetProperty("wednesday_close").GetString())
                        ),
                        Thursday = new OpeningHours(
                            TimeSpan.Parse(restaurantEl.GetProperty("thursday_open").GetString()),
                            TimeSpan.Parse(restaurantEl.GetProperty("thursday_close").GetString())
                        ),
                        Friday = new OpeningHours(
                            TimeSpan.Parse(restaurantEl.GetProperty("friday_open").GetString()),
                            TimeSpan.Parse(restaurantEl.GetProperty("friday_close").GetString())
                        ),
                        Saturday = new OpeningHours(
                            TimeSpan.Parse(restaurantEl.GetProperty("saturday_open").GetString()),
                            TimeSpan.Parse(restaurantEl.GetProperty("saturday_close").GetString())
                        ),
                        Sunday = new OpeningHours(
                            TimeSpan.Parse(restaurantEl.GetProperty("sunday_open").GetString()),
                            TimeSpan.Parse(restaurantEl.GetProperty("sunday_close").GetString())
                        )
                    };

                    var menuEl = restaurantEl.GetProperty("menu");

                    var menu = new Menu(restaurant.Id);

                    foreach (var categoryEl in menuEl.GetProperty("categories").EnumerateArray())
                    {
                        var categoryName = categoryEl.GetProperty("name").GetString();

                        menu.AddCategory(categoryName);

                        foreach (var itemEl in categoryEl.GetProperty("items").EnumerateArray())
                        {
                            menu.GetCategory(categoryName)
                                .AddItem(
                                    itemEl.GetProperty("name").GetString(),
                                    itemEl.GetProperty("description").GetString(),
                                    new Money(itemEl.GetProperty("price").GetDecimal())
                                );
                        }
                    }

                    var eventDto = new EventDto(
                        new RestaurantRegisteredEvent(restaurant.Id, user.Id)
                    );

                    await context.AddRangeAsync(user, restaurant, menu, eventDto);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
