using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebTruss.DependencyInjection
{
    public interface IServiceConfiguration
    {
        IServiceCollection AddInfrastructureServices(IServiceCollection services, IConfiguration configuration);
    }
}
