using Framework.BuildingBlock.HttpApi;
using Localization.Resources.AbpUi;
using PermissionService.Localization;
//using Volo.Abp.Account;
//using Volo.Abp.SettingManagement;
//using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.TenantManagement;

namespace PermissionService;

[DependsOn(
   typeof(PermissionServiceApplicationContractsModule),
   typeof(AbpPermissionManagementHttpApiModule),
   typeof(BuildingBlockHttpApiModule),
   //typeof(AbpSettingManagementHttpApiModule),
   //typeof(AbpFeatureManagementHttpApiModule),
   //typeof(AbpAccountHttpApiModule),
   typeof(AbpIdentityHttpApiModule),
   typeof(AbpTenantManagementHttpApiModule)
   )]
public class PermissionServiceHttpApiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        ConfigureLocalization();
    }

    private void ConfigureLocalization()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<PermissionServiceResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource)
                );
        });
    }
}
