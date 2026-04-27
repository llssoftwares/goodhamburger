namespace GoodHamburger.Web.Clients;

public static class HttpClientConfiguration
{
    extension(IServiceCollection services)
    {
        public IHttpClientBuilder AddApiHttpClient<TClient>(
        IConfiguration configuration)
        where TClient : class
        {
            var baseAddress = configuration["ApiUrl"] ?? "https+http://api";

            return services.AddHttpClient<TClient>(client =>
            {
                client.BaseAddress = new Uri(baseAddress);
            });
        }
    }
}