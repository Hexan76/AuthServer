using Framework.BuildingBlock.Application.Contracts;
using Framework.BuildingBlock.Permissions;
using Framework.Security;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace AuthServer
{
    [Dependency(ReplaceServices = true)]
    public class AuthServerPermissionDataSeedContributor(IPermissionClient PermissionClient) : IDataSeedContributor, ITransientDependency
    {
        public async Task SeedAsync(DataSeedContext context)
        {
            var permissions = typeof(AuthServerApplicationContractsModule).Assembly.GetDefinitionPermissions(nameof(AuthServer));

            PermissionSyncSnapshot snapshot = new PermissionSyncSnapshot
            {
                ServiceName = "AuthServer",
                Groups = permissions
            };
            string payload = JsonSerializer.Serialize(snapshot);
            await PermissionClient.SyncDefinitions("AuthServer",payload , PermissionSignatureHelper.Sign(payload));
        }
    }
}
