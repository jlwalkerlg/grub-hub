using System.Threading.Tasks;
using Infrastructure.Geocoding;
using SharedTests;
using Xunit;

namespace InfrastructureTests.External.Geocoding
{
    public class GoogleGeocoderTests
    {
        private readonly GoogleGeocoder geocoder;

        public GoogleGeocoderTests()
        {
            geocoder = new GoogleGeocoder(Config.GoogleGeocodingApiKey);
        }

        [Fact]
        public async Task It_Converts_An_Address_Into_Coordinates()
        {
            var result = await geocoder.Geocode("1 Maine Road, Manchester, UK");

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value.FormattedAddress);
            Assert.NotEqual(default(float), result.Value.Coordinates.Latitude);
            Assert.NotEqual(default(float), result.Value.Coordinates.Longitude);
        }

        [Fact]
        public async Task It_Returns_An_Error_If_Geocoding_Fails()
        {
            var coordinatesResult = await geocoder.Geocode("random");

            Assert.False(coordinatesResult.IsSuccess);
        }
    }
}
