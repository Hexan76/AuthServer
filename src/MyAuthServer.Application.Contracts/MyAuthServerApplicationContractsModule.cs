//using Volo.Abp.Account;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.TenantManagement;
using Framework.BuildingBlock.Application.Contracts;

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
}
