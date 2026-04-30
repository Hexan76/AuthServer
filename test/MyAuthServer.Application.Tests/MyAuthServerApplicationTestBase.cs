using Volo.Abp.Modularity;

namespace MyAuthServer;

public abstract class MyAuthServerApplicationTestBase<TStartupModule> : MyAuthServerTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
