using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
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

public class MyPermissionManagementProvider : UserPermissionManagementProvider 
{
    public MyPermissionManagementProvider(IPermissionGrantRepository permissionGrantRepository, IGuidGenerator guidGenerator, ICurrentTenant currentTenant) : base(permissionGrantRepository, guidGenerator, currentTenant)
    {
    }

    public override Task<PermissionValueProviderGrantInfo> CheckAsync(string name, string providerName, string providerKey)
    {
        return base.CheckAsync(name, providerName, providerKey);
    }

    public override async Task<MultiplePermissionValueProviderGrantInfo> CheckAsync(string[] names, string providerName, string providerKey)
    {
        //return await base.CheckAsync(names, providerName, providerKey);
        using (PermissionGrantRepository.DisableTracking())
        {
            MultiplePermissionValueProviderGrantInfo multiplePermissionValueProviderGrantInfo = new MultiplePermissionValueProviderGrantInfo(names);
            if (providerName != Name)
            {
                return multiplePermissionValueProviderGrantInfo;
            }
            List<PermissionGrant> source = await PermissionGrantRepository.GetListAsync(names, providerName, providerKey).ConfigureAwait(continueOnCapturedContext: false);
            foreach (string permissionName in names)
            {
                bool isGranted = source.Any((PermissionGrant x) => x.Name == permissionName);
                multiplePermissionValueProviderGrantInfo.Result[permissionName] = new PermissionValueProviderGrantInfo(isGranted, providerKey);
            }
            return multiplePermissionValueProviderGrantInfo;
        }

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

public class MyRolePermissionManagement : PermissionManagementProvider
{
    public override string Name => "R";
    public MyRolePermissionManagement(IPermissionGrantRepository permissionGrantRepository, IGuidGenerator guidGenerator, ICurrentTenant currentTenant) : base(permissionGrantRepository, guidGenerator, currentTenant)
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