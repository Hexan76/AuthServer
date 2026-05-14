using Framework.BuildingBlock.Domain;
using PermissionService.MultiTenancy;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.PermissionManagement.OpenIddict;
using Volo.Abp.Security.Claims;

namespace PermissionService;

[DependsOn(
    typeof(PermissionServiceDomainSharedModule),
    typeof(AbpAuditLoggingDomainModule),
    typeof(AbpCachingModule),
    typeof(BuildingBlockDomainModule),
    typeof(AbpBackgroundJobsDomainModule),
    //TODO: how to handle IdentityProvider without Remote
    //typeof(AbpPermissionManagementDomainIdentityModule),
    typeof(AbpPermissionManagementDomainOpenIddictModule)
    )]
public class PermissionServiceDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = MultiTenancyConsts.IsEnabled;
        });

        //TODO: Where can i put these config? Can be Configurable
        AbpClaimTypes.UserId = "sub";
        AbpClaimTypes.Role = "role";

    }
}
