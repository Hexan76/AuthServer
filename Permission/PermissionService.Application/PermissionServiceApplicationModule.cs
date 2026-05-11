using Volo.Abp.PermissionManagement;
//using Volo.Abp.SettingManagement;
//using Volo.Abp.FeatureManagement;
//using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.TenantManagement;

namespace PermissionService;

[DependsOn(
    typeof(PermissionServiceDomainModule),
    typeof(PermissionServiceApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    //typeof(AbpFeatureManagementApplicationModule),
    //typeof(AbpSettingManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    //typeof(AbpAccountApplicationModule),
    typeof(AbpTenantManagementApplicationModule)
    )]
public class PermissionServiceApplicationModule : AbpModule
{

}
