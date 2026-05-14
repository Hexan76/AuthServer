//using Volo.Abp.Account;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.TenantManagement;
using Framework.BuildingBlock.Application.Contracts;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Data;
using System.Data;
using Volo.Abp;
using System.Linq;

namespace MyAuthServer;

[DependsOn(
    typeof(MyAuthServerDomainSharedModule),
    typeof(AbpFeatureManagementApplicationContractsModule),
    typeof(AbpSettingManagementApplicationContractsModule),
    typeof(AbpIdentityApplicationContractsModule),
    //typeof(AbpAccountApplicationContractsModule),
    typeof(AbpTenantManagementApplicationContractsModule),
    typeof(BuildingBlockApplicationContractsModule)//,
                                                   //typeof(AbpPermissionManagementApplicationContractsModule)
)]
public class MyAuthServerApplicationContractsModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        MyAuthServerDtoExtensions.Configure();
    }

    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        using var scope = context.ServiceProvider.CreateScope();

        var seeder = scope.ServiceProvider.GetService<MyAuthServerPermissionDataSeedContributor>();

        //await seeder.SeedAsync(new DataSeedContext());
    }
}
