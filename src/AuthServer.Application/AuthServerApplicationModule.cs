//using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
//using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.Mapperly;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.TenantManagement;
using Framework.BuildingBlock.Application;

namespace AuthServer;

[DependsOn(
    typeof(AuthServerDomainModule),
    typeof(AuthServerApplicationContractsModule),
    //typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    //typeof(AbpAccountApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(BuildingBlockApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class AuthServerApplicationModule : AbpModule
{

}
