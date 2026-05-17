using Volo.Abp.Modularity;

namespace AuthServer;

[DependsOn(
    typeof(AuthServerApplicationModule),
    typeof(AuthServerDomainTestModule)
)]
public class AuthServerApplicationTestModule : AbpModule
{

}
