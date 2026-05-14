using Framework.BuildingBlock.Application.Contracts;
using Framework.BuildingBlock.Permissions;
using Framework.Security;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace MyAuthServer
{
    [Dependency(ReplaceServices = true)]
    public class MyAuthServerPermissionDataSeedContributor(IPermissionClient PermissionClient) : IDataSeedContributor, ITransientDependency
    {
        public async Task SeedAsync(DataSeedContext context)
        {
            var permissions = typeof(MyAuthServerApplicationContractsModule).Assembly.GetDefinitionPermissions(nameof(MyAuthServer));

            PermissionSyncSnapshot snapshot = new PermissionSyncSnapshot
            {
                ServiceName = "MyAuthServer",
                Groups = permissions
            };
            string payload = JsonSerializer.Serialize(snapshot);
            await PermissionClient.SyncDefinitions("MyAuthServer",payload , PermissionSignatureHelper.Sign(payload));
        }
    }
}
