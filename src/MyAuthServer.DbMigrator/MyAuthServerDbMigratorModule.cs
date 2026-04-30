using MyAuthServer.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace MyAuthServer.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(MyAuthServerEntityFrameworkCoreModule),
    typeof(MyAuthServerApplicationContractsModule)
)]
public class MyAuthServerDbMigratorModule : AbpModule
{
}
