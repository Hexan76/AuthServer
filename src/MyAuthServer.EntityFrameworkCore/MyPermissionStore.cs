using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.PermissionManagement;

namespace MyAuthServer;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IPermissionStore))]
public class MyPermissionStore : IPermissionStore, ITransientDependency
{
    public Task<bool> IsGrantedAsync(string name, string providerName, string providerKey)
    {
        throw new System.NotImplementedException();
    }

    public Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names, string providerName, string providerKey)
    {
        throw new System.NotImplementedException();
    }
}
