using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FoodSnap.Application.Restaurants;
using FoodSnap.Application.Services;
using FoodSnap.Domain;
using FoodSnap.Domain.Restaurants;

namespace FoodSnap.Infrastructure.Persistence.Dapper.Repositories.Restaurants
{
    public class DPRestaurantDtoRepository : IRestaurantDtoRepository
    {
        private readonly IDbConnectionFactory dbConnectionFactory;
        private readonly IClock clock;

        public DPRestaurantDtoRepository(
            IDbConnectionFactory dbConnectionFactory, IClock clock)
        {
            this.dbConnectionFactory = dbConnectionFactory;
            this.clock = clock;
        }

        public async Task<RestaurantDto> GetById(Guid id)
        {
            var sql = @"
                SELECT
                    r.id,
                    r.manager_id,
                    r.name,
                    r.phone_number,
                    r.address,
                    r.latitude,
                    r.longitude,
                    r.status,
                    r.monday_open,
                    r.monday_close,
                    r.tuesday_open,
                    r.tuesday_close,
                    r.wednesday_open,
                    r.wednesday_close,
                    r.thursday_open,
                    r.thursday_close,
                    r.friday_open,
                    r.friday_close,
                    r.saturday_open,
                    r.saturday_close,
                    r.sunday_open,
                    r.sunday_close,
                    r.delivery_fee,
                    r.minimum_delivery_spend,
                    r.max_delivery_distance_in_km,
                    r.estimated_delivery_time_in_minutes
                FROM
                    restaurants r
                WHERE
                    r.id = @Id";

            using (var connection = await dbConnectionFactory.OpenConnection())
            {
                var entry = await connection
                    .QuerySingleOrDefaultAsync<RestaurantEntry>(
                        sql,
                        new { Id = id });

                if (entry == null)
                {
                    return null;
                }

                return EntryToDto(entry);
            }
        }

        public async Task<List<RestaurantDto>> Search(Coordinates coordinates)
        {
            var sql = @"
                SELECT
                    r.id,
                    r.manager_id,
                    r.name,
                    r.phone_number,
                    r.address,
                    r.latitude,
                    r.longitude,
                    r.status,
                    r.monday_open,
                    r.monday_close,
                    r.tuesday_open,
                    r.tuesday_close,
                    r.wednesday_open,
                    r.wednesday_close,
                    r.thursday_open,
                    r.thursday_close,
                    r.friday_open,
                    r.friday_close,
                    r.saturday_open,
                    r.saturday_close,
                    r.sunday_open,
                    r.sunday_close,
                    r.delivery_fee,
                    r.minimum_delivery_spend,
                    r.max_delivery_distance_in_km,
                    r.estimated_delivery_time_in_minutes
                FROM
                    restaurants r
                WHERE
                    r.status = @Status";

            var now = clock.UtcNow;
            var day = now.DayOfWeek.ToString().ToLower();

            sql += $" AND {day}_open < @Now AND {day}_close > @Now";

            sql += " AND FLOOR(6371000 * acos(sin(radians(r.latitude)) * sin(radians(@OriginLatitude)) + cos(radians(r.latitude)) * cos(radians(@OriginLatitude)) * cos(radians(@OriginLongitude - r.longitude)))) / 1000 <= r.max_delivery_distance_in_km";

            using (var connection = await dbConnectionFactory.OpenConnection())
            {
                var entries = await connection
                    .QueryAsync<RestaurantEntry>(
                        sql,
                        new
                        {
                            Status = RestaurantStatus.Approved.ToString(),
                            Now = new TimeSpan(now.Hour, now.Minute, 0),
                            OriginLatitude = coordinates.Latitude,
                            OriginLongitude = coordinates.Longitude,
                        });

                return entries.Select(EntryToDto).ToList();
            }
        }

        private RestaurantDto EntryToDto(RestaurantEntry entry)
        {
            return new RestaurantDto()
            {
                Id = entry.Id,
                ManagerId = entry.ManagerId,
                Name = entry.Name,
                PhoneNumber = entry.PhoneNumber,
                Address = entry.Address,
                Latitude = entry.Latitude,
                Longitude = entry.Longitude,
                Status = entry.Status,
                OpeningTimes = new OpeningTimesDto()
                {
                    Monday = entry.MondayOpen.HasValue ? new OpeningHoursDto()
                    {
                        Open = FormatTimeSpan(entry.MondayOpen.Value),
                        Close = FormatTimeSpan(entry.MondayClose.Value),
                    } : null,
                    Tuesday = entry.TuesdayOpen.HasValue ? new OpeningHoursDto()
                    {
                        Open = FormatTimeSpan(entry.TuesdayOpen.Value),
                        Close = FormatTimeSpan(entry.TuesdayClose.Value),
                    } : null,
                    Wednesday = entry.WednesdayOpen.HasValue ? new OpeningHoursDto()
                    {
                        Open = FormatTimeSpan(entry.WednesdayOpen.Value),
                        Close = FormatTimeSpan(entry.WednesdayClose.Value),
                    } : null,
                    Thursday = entry.ThursdayOpen.HasValue ? new OpeningHoursDto()
                    {
                        Open = FormatTimeSpan(entry.ThursdayOpen.Value),
                        Close = FormatTimeSpan(entry.ThursdayClose.Value),
                    } : null,
                    Friday = entry.FridayOpen.HasValue ? new OpeningHoursDto()
                    {
                        Open = FormatTimeSpan(entry.FridayOpen.Value),
                        Close = FormatTimeSpan(entry.FridayClose.Value),
                    } : null,
                    Saturday = entry.SaturdayOpen.HasValue ? new OpeningHoursDto()
                    {
                        Open = FormatTimeSpan(entry.SaturdayOpen.Value),
                        Close = FormatTimeSpan(entry.SaturdayClose.Value),
                    } : null,
                    Sunday = entry.SundayOpen.HasValue ? new OpeningHoursDto()
                    {
                        Open = FormatTimeSpan(entry.SundayOpen.Value),
                        Close = FormatTimeSpan(entry.SundayClose.Value),
                    } : null,
                },
                DeliveryFee = entry.DeliveryFee,
                MinimumDeliverySpend = entry.MinimumDeliverySpend,
                MaxDeliveryDistanceInKm = entry.MaxDeliveryDistanceInKm,
                EstimatedDeliveryTimeInMinutes = entry.EstimatedDeliveryTimeInMinutes,
            };
        }

        private string FormatTimeSpan(TimeSpan t)
        {
            return $"{t.Hours.ToString().PadLeft(2, '0')}:{t.Minutes.ToString().PadLeft(2, '0')}";
        }

        private record RestaurantEntry
        {
            public Guid Id { get; init; }
            public Guid ManagerId { get; init; }
            public string Name { get; init; }
            public string PhoneNumber { get; init; }
            public string Address { get; init; }
            public float Latitude { get; init; }
            public float Longitude { get; init; }
            public string Status { get; init; }
            public TimeSpan? MondayOpen { get; init; }
            public TimeSpan? MondayClose { get; init; }
            public TimeSpan? TuesdayOpen { get; init; }
            public TimeSpan? TuesdayClose { get; init; }
            public TimeSpan? WednesdayOpen { get; init; }
            public TimeSpan? WednesdayClose { get; init; }
            public TimeSpan? ThursdayOpen { get; init; }
            public TimeSpan? ThursdayClose { get; init; }
            public TimeSpan? FridayOpen { get; init; }
            public TimeSpan? FridayClose { get; init; }
            public TimeSpan? SaturdayOpen { get; init; }
            public TimeSpan? SaturdayClose { get; init; }
            public TimeSpan? SundayOpen { get; init; }
            public TimeSpan? SundayClose { get; init; }
            public decimal DeliveryFee { get; init; }
            public decimal MinimumDeliverySpend { get; init; }
            public int MaxDeliveryDistanceInKm { get; init; }
            public int EstimatedDeliveryTimeInMinutes { get; init; }
        }
    }
}
