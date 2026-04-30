using Volo.Abp.Modularity;

namespace MyAuthServer;

[DependsOn(
    typeof(MyAuthServerApplicationModule),
    typeof(MyAuthServerDomainTestModule)
)]
public class MyAuthServerApplicationTestModule : AbpModule
{

}
