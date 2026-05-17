using Volo.Abp.Modularity;

namespace AuthServer;

public abstract class AuthServerApplicationTestBase<TStartupModule> : AuthServerTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
