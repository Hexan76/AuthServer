using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Volo.Abp.SimpleStateChecking;

namespace PermissionService;

[Dependency(ReplaceServices = true)]
public class MyPermissionChecker : PermissionChecker
{
    protected IPermissionDefinitionManager PermissionDefinitionManager { get; }

    protected ICurrentPrincipalAccessor PrincipalAccessor { get; }

    protected ICurrentTenant CurrentTenant { get; }

    protected IPermissionValueProviderManager PermissionValueProviderManager { get; }

    protected ISimpleStateCheckerManager<PermissionDefinition> StateCheckerManager { get; }
    public MyPermissionChecker(
        IPermissionDefinitionManager permissionDefinitionManager,
        ICurrentPrincipalAccessor principalAccessor,
        ICurrentTenant currentTenant,
        IPermissionValueProviderManager permissionValueProviderManager,
        ISimpleStateCheckerManager<PermissionDefinition> stateCheckerManager) : base(principalAccessor, permissionDefinitionManager, currentTenant, permissionValueProviderManager, stateCheckerManager)
    {
        PermissionDefinitionManager = permissionDefinitionManager;
        PrincipalAccessor = principalAccessor;
        CurrentTenant = currentTenant;
        PermissionValueProviderManager = permissionValueProviderManager;
        StateCheckerManager = stateCheckerManager;
    }


    public override async Task<bool> IsGrantedAsync(ClaimsPrincipal? claimsPrincipal, string name)
    {
        Check.NotNull<string>(name, "name");
        PermissionDefinition permission = await PermissionDefinitionManager.GetOrNullAsync(name).ConfigureAwait(continueOnCapturedContext: false);
        if (permission == null)
        {
            return false;
        }
        if (!permission.IsEnabled)
        {
            return false;
        }
        if (!(await StateCheckerManager.IsEnabledAsync(permission).ConfigureAwait(continueOnCapturedContext: false)))
        {
            return false;
        }
        MultiTenancySides multiTenancySides = claimsPrincipal?.GetMultiTenancySide() ?? CurrentTenant.GetMultiTenancySide();
        if (!permission.MultiTenancySide.HasFlag(multiTenancySides))
        {
            return false;
        }
        bool isGranted = false;
        PermissionValueCheckContext context = new PermissionValueCheckContext(permission, claimsPrincipal);
        foreach (IPermissionValueProvider valueProvider in PermissionValueProviderManager.ValueProviders)
        {
            if (!context.Permission.Providers.Any() || context.Permission.Providers.Contains(valueProvider.Name))
            {
                switch (await valueProvider.CheckAsync(context).ConfigureAwait(continueOnCapturedContext: false))
                {
                    case PermissionGrantResult.Granted:
                        isGranted = true;
                        break;
                    case PermissionGrantResult.Prohibited:
                        return false;
                }
            }
        }
        return isGranted;
    }

    public override Task<MultiplePermissionGrantResult> IsGrantedAsync(ClaimsPrincipal? claimsPrincipal, string[] names)
    {
        return base.IsGrantedAsync(claimsPrincipal, names);
    }

    public override Task<bool> IsGrantedAsync(string name)
    {
        return base.IsGrantedAsync(name);
    }

 
}
