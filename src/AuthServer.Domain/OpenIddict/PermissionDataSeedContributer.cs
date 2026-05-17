using Framework.BuildingBlock.Permissions;
using Framework.Security;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace AuthServer.OpenIddict;

/* Creates initial data that is needed to property run the application
 * and make client-to-server communication possible.
 */
[ExposeServices(typeof(IDataSeedContributor),typeof(PermissionDataSeedContributer))]
public class PermissionDataSeedContributer : IDataSeedContributor, ITransientDependency
{
    public async Task SeedAsync(DataSeedContext context)
    {
        //await permissionClient.SyncDefinitions("", "", PermissionSignatureHelper.Sign("123"));
    }
}