using Autofac;
using Web.Services.Geocoding;

namespace Web.ServiceRegistration
{
    public static class GeocoderRegistrar
    {
        public static void AddGeocoder(this ContainerBuilder builder, WebConfig config)
        {
            builder
                .Register(ctx => new GoogleGeocoder(config.GoogleGeocodingApiKey))
                .As<IGeocoder>()
                .SingleInstance();
        }
    }
}
