using Volo.Abp.Modularity;

namespace MyAuthServer;

/* Inherit from this class for your domain layer tests. */
public abstract class MyAuthServerDomainTestBase<TStartupModule> : MyAuthServerTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
