using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Web.Data.Dapper.Repositories.Restaurants;
using Web.Domain;
using Web.Features.Restaurants;
using WebTests.Doubles;
using WebTests.TestData;
using Xunit;

namespace WebTests.Data.Dapper.Repositories.Restaurants
{
    public class DPRestaurantDtoRepositoryTests : IntegrationTestBase
    {
        private readonly ClockStub clock;
        private readonly DPRestaurantDtoRepository repository;

        public DPRestaurantDtoRepositoryTests(IntegrationTestFixture fixture) : base(fixture)
        {
            clock = new ClockStub();

            repository = new DPRestaurantDtoRepository(
                new TestDbConnectionFactory(),
                clock
            );
        }

        [Fact]
        public async Task It_Retrieves_Open_Restaurants_Within_Range()
        {
            clock.UtcNow = DateTime.Parse("Tue, 15 Mar 2005 12:00:00 GMT");

            var m1 = new User();
            var m2 = new User();
            var m3 = new User();
            var m4 = new User();

            var italian = new Cuisine { Name = "Italian" };

            // not approved
            var r1 = new Restaurant()
            {
                ManagerId = m1.Id,
                Latitude = 54.0f,
                Longitude = -2.0f,
                MaxDeliveryDistanceInKm = 5,
                TuesdayOpen = TimeSpan.Zero,
                TuesdayClose = null,
                Status = "PendingApproval",
            };

            // not open
            var r2 = new Restaurant()
            {
                ManagerId = m2.Id,
                Latitude = 54.0f,
                Longitude = -2.0f,
                MaxDeliveryDistanceInKm = 5,
                TuesdayOpen = null,
                TuesdayClose = null,
                Status = "Approved",
            };

            // out of range
            var r3 = new Restaurant()
            {
                ManagerId = m3.Id,
                Latitude = 55.0f,
                Longitude = -2.0f,
                MaxDeliveryDistanceInKm = 5,
                TuesdayOpen = TimeSpan.Zero,
                TuesdayClose = null,
                Status = "Approved",
            };

            // expected
            var r4 = new Restaurant()
            {
                ManagerId = m4.Id,
                Latitude = 54.0f,
                Longitude = -2.0f,
                MaxDeliveryDistanceInKm = 5,
                TuesdayOpen = TimeSpan.Zero,
                TuesdayClose = null,
                Status = "Approved",
                Cuisines = new() { italian },
            };

            fixture.Insert(m1, m2, m3, m4, r1, r2, r3, r4, italian);

            var restaurants = await repository.Search(new Coordinates(54.0f, -2.0f));

            restaurants.ShouldHaveSingleItem();
            restaurants.Single().Id.ShouldBe(r4.Id);
        }

        [Fact]
        public async Task It_Sorts_Restaurants_By_Distance()
        {
            clock.UtcNow = DateTime.Parse("Tue, 15 Mar 2005 12:00:00 GMT");

            var m1 = new User();
            var m2 = new User();
            var m3 = new User();

            var r1 = new Restaurant()
            {
                ManagerId = m1.Id,
                Latitude = 54.0f,
                Longitude = -2.15f,
                MaxDeliveryDistanceInKm = 10,
                TuesdayOpen = TimeSpan.Zero,
                TuesdayClose = null,
            };

            var r2 = new Restaurant()
            {
                ManagerId = m2.Id,
                Latitude = 54.0f,
                Longitude = -2.0f,
                MaxDeliveryDistanceInKm = 10,
                TuesdayOpen = TimeSpan.Zero,
                TuesdayClose = null,
            };

            var r3 = new Restaurant()
            {
                ManagerId = m3.Id,
                Latitude = 54.0f,
                Longitude = -2.1f,
                MaxDeliveryDistanceInKm = 10,
                TuesdayOpen = TimeSpan.Zero,
                TuesdayClose = null,
            };

            fixture.Insert(m1, m2, m3, r1, r2, r3);

            var restaurants = await repository.Search(
                new Coordinates(54.0f, -2.0f),
                new RestaurantSearchOptions()
                {
                    SortBy = "distance",
                });

            restaurants.Count.ShouldBe(3);
            restaurants[0].Id.ShouldBe(r2.Id);
            restaurants[1].Id.ShouldBe(r3.Id);
            restaurants[2].Id.ShouldBe(r1.Id);
        }

        [Fact]
        public async Task It_Sorts_Restaurants_By_Min_Order()
        {
            clock.UtcNow = DateTime.Parse("Tue, 15 Mar 2005 12:00:00 GMT");

            var m1 = new User();
            var m2 = new User();
            var m3 = new User();

            var r1 = new Restaurant()
            {
                ManagerId = m1.Id,
                Latitude = 54.0f,
                Longitude = -2.0f,
                MaxDeliveryDistanceInKm = 10,
                MinimumDeliverySpend = 15.00m,
                TuesdayOpen = TimeSpan.Zero,
                TuesdayClose = null,
            };

            var r2 = new Restaurant()
            {
                ManagerId = m2.Id,
                Latitude = 54.0f,
                Longitude = -2.0f,
                MaxDeliveryDistanceInKm = 10,
                MinimumDeliverySpend = 6.00m,
                TuesdayOpen = TimeSpan.Zero,
                TuesdayClose = null,
            };

            var r3 = new Restaurant()
            {
                ManagerId = m3.Id,
                Latitude = 54.0f,
                Longitude = -2.0f,
                MaxDeliveryDistanceInKm = 10,
                MinimumDeliverySpend = 10.00m,
                TuesdayOpen = TimeSpan.Zero,
                TuesdayClose = null,
            };

            fixture.Insert(m1, m2, m3, r1, r2, r3);

            var restaurants = await repository.Search(
                new Coordinates(54.0f, -2.0f),
                new RestaurantSearchOptions()
                {
                    SortBy = "min_order",
                });

            restaurants.Count.ShouldBe(3);
            restaurants[0].Id.ShouldBe(r2.Id);
            restaurants[1].Id.ShouldBe(r3.Id);
            restaurants[2].Id.ShouldBe(r1.Id);
        }

        [Fact]
        public async Task It_Sorts_Restaurants_By_Delivery_Fee()
        {
            clock.UtcNow = DateTime.Parse("Tue, 15 Mar 2005 12:00:00 GMT");

            var m1 = new User();
            var m2 = new User();
            var m3 = new User();

            var r1 = new Restaurant()
            {
                ManagerId = m1.Id,
                Latitude = 54.0f,
                Longitude = -2.0f,
                MaxDeliveryDistanceInKm = 10,
                TuesdayOpen = TimeSpan.Zero,
                TuesdayClose = null,
                DeliveryFee = 3.00m,
            };

            var r2 = new Restaurant()
            {
                ManagerId = m2.Id,
                Latitude = 54.0f,
                Longitude = -2.0f,
                MaxDeliveryDistanceInKm = 10,
                TuesdayOpen = TimeSpan.Zero,
                TuesdayClose = null,
                DeliveryFee = 0,
            };

            var r3 = new Restaurant()
            {
                ManagerId = m3.Id,
                Latitude = 54.0f,
                Longitude = -2.0f,
                MaxDeliveryDistanceInKm = 10,
                TuesdayOpen = TimeSpan.Zero,
                TuesdayClose = null,
                DeliveryFee = 1.50m,
            };


            fixture.Insert(m1, m2, m3, r1, r2, r3);

            var restaurants = await repository.Search(
                new Coordinates(54.0f, -2.0f),
                new RestaurantSearchOptions()
                {
                    SortBy = "delivery_fee",
                });

            restaurants.Count.ShouldBe(3);
            restaurants[0].Id.ShouldBe(r2.Id);
            restaurants[1].Id.ShouldBe(r3.Id);
            restaurants[2].Id.ShouldBe(r1.Id);
        }

        [Fact]
        public async Task It_Sorts_Restaurants_By_Time()
        {
            clock.UtcNow = DateTime.Parse("Tue, 15 Mar 2005 12:00:00 GMT");

            var m1 = new User();
            var m2 = new User();
            var m3 = new User();

            var r1 = new Restaurant()
            {
                ManagerId = m1.Id,
                Latitude = 54.0f,
                Longitude = -2.0f,
                MaxDeliveryDistanceInKm = 10,
                TuesdayOpen = TimeSpan.Zero,
                TuesdayClose = null,
                EstimatedDeliveryTimeInMinutes = 60,
            };

            var r2 = new Restaurant()
            {
                ManagerId = m2.Id,
                Latitude = 54.0f,
                Longitude = -2.0f,
                MaxDeliveryDistanceInKm = 10,
                TuesdayOpen = TimeSpan.Zero,
                TuesdayClose = null,
                EstimatedDeliveryTimeInMinutes = 30,
            };

            var r3 = new Restaurant()
            {
                ManagerId = m3.Id,
                Latitude = 54.0f,
                Longitude = -2.0f,
                MaxDeliveryDistanceInKm = 10,
                TuesdayOpen = TimeSpan.Zero,
                TuesdayClose = null,
                EstimatedDeliveryTimeInMinutes = 40,
            };

            fixture.Insert(m1, m2, m3, r1, r2, r3);

            var restaurants = await repository.Search(
                new Coordinates(54.0f, -2.0f),
                new RestaurantSearchOptions()
                {
                    SortBy = "time",
                });

            restaurants.Count.ShouldBe(3);
            restaurants[0].Id.ShouldBe(r2.Id);
            restaurants[1].Id.ShouldBe(r3.Id);
            restaurants[2].Id.ShouldBe(r1.Id);
        }

        [Fact]
        public async Task It_Filters_Restaurants_By_Cuisine()
        {
            clock.UtcNow = DateTime.Parse("Tue, 15 Mar 2005 12:00:00 GMT");

            var m1 = new User();
            var m2 = new User();
            var m3 = new User();

            var thai = new Cuisine() { Name = "Thai" };
            var italian = new Cuisine() { Name = "Italian" };
            var indian = new Cuisine() { Name = "Indian" };

            var r1 = new Restaurant()
            {
                ManagerId = m1.Id,
                Latitude = 54.0f,
                Longitude = -2.0f,
                MaxDeliveryDistanceInKm = 10,
                TuesdayOpen = TimeSpan.Zero,
                TuesdayClose = null,
                Cuisines = new() { thai, italian },
            };

            var r2 = new Restaurant()
            {
                ManagerId = m2.Id,
                Latitude = 54.0f,
                Longitude = -2.0f,
                MaxDeliveryDistanceInKm = 10,
                TuesdayOpen = TimeSpan.Zero,
                TuesdayClose = null,
                Cuisines = new() { thai },
            };

            var r3 = new Restaurant()
            {
                ManagerId = m3.Id,
                Latitude = 54.0f,
                Longitude = -2.0f,
                MaxDeliveryDistanceInKm = 10,
                TuesdayOpen = TimeSpan.Zero,
                TuesdayClose = null,
                Cuisines = new() { indian },
            };

            fixture.Insert(m1, m2, m3, r1, r2, r3);

            var restaurants = await repository.Search(
                new Coordinates(54.0f, -2.0f),
                new RestaurantSearchOptions()
                {
                    Cuisines = new() { "Thai", "Italian" }
                });

            restaurants.Count.ShouldBe(2);
            restaurants.ShouldContain(x => x.Id == r1.Id);
            restaurants.ShouldContain(x => x.Id == r2.Id);
        }
    }
}
