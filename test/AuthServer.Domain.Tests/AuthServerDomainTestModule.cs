using Volo.Abp.Modularity;

namespace AuthServer;

[DependsOn(
    typeof(AuthServerDomainModule),
    typeof(AuthServerTestBaseModule)
)]
public class AuthServerDomainTestModule : AbpModule
{

}
