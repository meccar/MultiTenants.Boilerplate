using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection ConfigureBuildingBlocksCoreDependencyInjection(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            return services;
        }
    }
}
