//using Volo.Abp.Account;
using Framework.BuildingBlock.Application.Contracts;
//using Volo.Abp.SettingManagement;
//using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement;

namespace PermissionService;

[DependsOn(
    typeof(PermissionServiceDomainSharedModule),
    //typeof(AbpFeatureManagementApplicationContractsModule),
    //typeof(AbpSettingManagementApplicationContractsModule),
    typeof(AbpIdentityApplicationContractsModule),
    typeof(BuildingBlockApplicationContractsModule),
    //typeof(AbpAccountApplicationContractsModule),
    typeof(AbpTenantManagementApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationContractsModule)
)]
public class PermissionServiceApplicationContractsModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PermissionServiceDtoExtensions.Configure();
    }
}
