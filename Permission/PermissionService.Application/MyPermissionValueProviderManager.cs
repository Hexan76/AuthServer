using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.Identity;
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
    public override string Name => "U";
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

public class MyPermissionManagementProvider : UserPermissionManagementProvider , IPermissionManagementProvider 
{
    public MyPermissionManagementProvider(IPermissionGrantRepository permissionGrantRepository, IGuidGenerator guidGenerator, ICurrentTenant currentTenant) : base(permissionGrantRepository, guidGenerator, currentTenant)
    {
    }

    public override Task<PermissionValueProviderGrantInfo> CheckAsync(string name, string providerName, string providerKey)
    {
        return base.CheckAsync(name, providerName, providerKey);
    }

    public override Task<MultiplePermissionValueProviderGrantInfo> CheckAsync(string[] names, string providerName, string providerKey)
    {
        return base.CheckAsync(names, providerName, providerKey);
    }

    protected override Task GrantAsync(string name, string providerKey)
    {
        return base.GrantAsync(name, providerKey);
    }

    public override Task SetAsync(string name, string providerKey, bool isGranted)
    {
        return base.SetAsync(name, providerKey, isGranted);
    }
}

public class MyRolePermissionManagement : RolePermissionManagementProvider, IPermissionManagementProvider
{
    public MyRolePermissionManagement(IPermissionGrantRepository permissionGrantRepository, IGuidGenerator guidGenerator, ICurrentTenant currentTenant, IUserRoleFinder userRoleFinder) : base(permissionGrantRepository, guidGenerator, currentTenant, userRoleFinder)
    {
    }

    public override Task<PermissionValueProviderGrantInfo> CheckAsync(string name, string providerName, string providerKey)
    {
        return base.CheckAsync(name, providerName, providerKey);
    }

    public override Task<MultiplePermissionValueProviderGrantInfo> CheckAsync(string[] names, string providerName, string providerKey)
    {
        return base.CheckAsync(names, providerName, providerKey);
    }

    protected override Task GrantAsync(string name, string providerKey)
    {
        return base.GrantAsync(name, providerKey);
    }

    public override Task SetAsync(string name, string providerKey, bool isGranted)
    {
        return base.SetAsync(name, providerKey, isGranted);
    }
}