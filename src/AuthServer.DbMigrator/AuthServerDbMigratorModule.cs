using AuthServer.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace AuthServer.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AuthServerEntityFrameworkCoreModule),
    typeof(AuthServerApplicationContractsModule)
)]
public class AuthServerDbMigratorModule : AbpModule
{
}
