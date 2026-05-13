using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

namespace PermissionService;

[Dependency(ReplaceServices = true)]
public class MyPermissionValueProviderManager : PermissionValueProviderManager
{
    public MyPermissionValueProviderManager(IServiceProvider serviceProvider, IOptions<AbpPermissionOptions> options) : base(serviceProvider, options)
    {
    }

    protected override List<IPermissionValueProvider> GetProviders()
    {
        return base.GetProviders();
    }
}

[Dependency(ReplaceServices = true)]
public class MyUserPermissionValueProvider : UserPermissionValueProvider
{
    public MyUserPermissionValueProvider(IPermissionStore permissionStore) : base(permissionStore)
    {
    }

    public override async Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context)
    {
        string text = context.Principal?.FindFirst(AbpClaimTypes.UserId)?.Value;
        if (text == null)
        {
            return PermissionGrantResult.Undefined;
        }
        return (await base.PermissionStore.IsGrantedAsync(context.Permission.Name, Name, text).ConfigureAwait(continueOnCapturedContext: false)) ? PermissionGrantResult.Granted : PermissionGrantResult.Undefined;
    }

    public override Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context)
    {
        return base.CheckAsync(context);
    }
}