using System.Diagnostics.Metrics;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using grpc_service;
using grpc_service.Protos;

namespace grpc_service.Services
{

    public class DataService : CountryCityService.CountryCityServiceBase
    {
        private readonly ILogger<DataService> _logger;

        public DataService(ILogger<DataService> logger)
        {
            _logger = logger;
        }

        private static List<Country> Countries = new List<Country>() {

            new Country(){ CountryId=1, CountryName="Germany" },
            new Country(){ CountryId=2, CountryName="Italy"}
        };

        private static List<City> Cities = new List<City>() {
            new City(){ CityId=1, CityName="Stuttgart", CountryId=1, Population=10000 },
            new City(){ CityId=2, CityName="Munchen", CountryId=1, Population=5324 },
            new City(){ CityId=3, CityName="Bari", CountryId=2, Population=104000 },
            new City(){ CityId=4, CityName="Milano", CountryId=2, Population=456456 },
            new City(){ CityId=5, CityName="Rome", CountryId=2, Population=242334 }
        };


        public override async Task GetCountries(Empty request, IServerStreamWriter<CountryResponse> responseStream, ServerCallContext context)
        {
            foreach (var country in Countries)
            {
                await responseStream.WriteAsync(new CountryResponse() { Name = country.CountryName });
            }
        }

        public override async Task GetCities(CountryRequest request, IServerStreamWriter<CityResponse> responseStream, ServerCallContext context)
        {
            var cityList = Cities.Where(x => x.CountryId == request.Countryid)
                .Select(x => new CityResponse() { Name = x.CityName, Countryid = x.CountryId });

            foreach (var city in cityList)
            {
                await responseStream.WriteAsync(city);
            }
        }

        public override Task<CityDetailResponse> GetCityDetail(CityDetailRequest request, ServerCallContext context)
        {
            var city = Cities
                .Where(x => x.CountryId == request.Cityid)
                .Select(x => new CityDetailResponse() { Cityid = x.CityId, Cityname = x.CityName, Citypopulation = x.Population })
                .FirstOrDefault();

            if (city == null)
                throw new ArgumentNullException("City could not be found.");

            return Task.FromResult<CityDetailResponse>(city);
        }
    }

}