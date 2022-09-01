using Grpc.Core;
using Grpc.Net.Client;
using grpc_service.Protos;

namespace grpc_console_client
{

    public class Program
    {
        static async Task Main(string[] args)
        {

            var channel = GrpcChannel.ForAddress("http://localhost:5001");
            var client = new CountryCityService.CountryCityServiceClient(channel);

            //Country List - Server Stream
            var countryResult = client.GetCountries(new Google.Protobuf.WellKnownTypes.Empty());

            while (await countryResult.ResponseStream.MoveNext())
            {
                Console.WriteLine("Country: " + countryResult.ResponseStream.Current.Name);
            }

            //City List - Server Stream
            var cityResult = client.GetCities(new CountryRequest() { Countryid = 1 });

            while (await cityResult.ResponseStream.MoveNext())
            {
                Console.WriteLine("City: " + cityResult.ResponseStream.Current.Name);
            }

            //City Detail - Unary
            var cityDetailResult = client.GetCityDetail(new CityDetailRequest() { Cityid = 1 });
            Console.WriteLine($"City Detail: {cityDetailResult.Cityname} {cityDetailResult.Citypopulation}");

            Console.ReadLine();
        }
    }

}
