//namespace Morsley.UK.People.API.Security.System.Tests;

//internal abstract class SecurityFixture
//{
//    protected readonly HttpClient HttpClient;

//    protected SecurityFixture()
//    {
//        var factory = new WebApplicationFactory<SecurityProgram>()
//            .WithWebHostBuilder(_ =>
//            {
//                _.UseKestrel();
//                _.ConfigureKestrel(_ =>
//                    {
//                        _.ListenLocalhost(5000);
//                    }
//                );
//                //_.ConfigureServices();
//            });
//        HttpClient = factory.CreateClient();
//    }
//}
