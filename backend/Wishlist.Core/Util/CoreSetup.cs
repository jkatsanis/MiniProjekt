using Wishlist.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Wishlist.Core.Util;

public static class CoreSetup
{
    public static void ConfigureCore(this IServiceCollection services)
    {
        services.AddSingleton<IClock>(SystemClock.Instance);
        
        services.AddScoped<IWishlistService, WishlistService>();
    }
}
