using PermissionService.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace PermissionService.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(PermissionServiceEntityFrameworkCoreModule),
    typeof(PermissionServiceApplicationContractsModule)
)]
public class PermissionServiceDbMigratorModule : AbpModule
{
}
