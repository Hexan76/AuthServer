using Volo.Abp.Modularity;

namespace MyAuthServer;

[DependsOn(
    typeof(MyAuthServerDomainModule),
    typeof(MyAuthServerTestBaseModule)
)]
public class MyAuthServerDomainTestModule : AbpModule
{

}
