using Framework.BuildingBlock.Application;
using Volo.Abp.Authorization.Permissions;

//using Volo.Abp.SettingManagement;
//using Volo.Abp.FeatureManagement;
//using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.TenantManagement;

namespace PermissionService;

[DependsOn(
    typeof(PermissionServiceDomainModule),
    typeof(PermissionServiceApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(BuildingBlockApplicationModule)
    //typeof(AbpFeatureManagementApplicationModule),
    //typeof(AbpSettingManagementApplicationModule),
    //typeof(AbpIdentityApplicationModule),
    //typeof(AbpAccountApplicationModule),
    //typeof(AbpTenantManagementApplicationModule)
    )]
public class PermissionServiceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<PermissionManagementOptions>(options =>
        {
            options.ManagementProviders.Add(typeof(MyPermissionManagementProvider));
            options.ManagementProviders.Add(typeof(MyRolePermissionManagement));
            options.ProviderPolicies[UserPermissionValueProvider.ProviderName] = "AbpIdentity.Users.ManagePermissions";
            options.ProviderPolicies[RolePermissionValueProvider.ProviderName] = "AbpIdentity.Roles.ManagePermissions";

        });

        //Configure<AbpPermissionOptions>(options =>
        //{
        //    options.ValueProviders.Remove<UserPermissionValueProvider>();
        //    options.ValueProviders.Insert(0, typeof(MyUserPermissionValueProvider));
        //});
    }
}
