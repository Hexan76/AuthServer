//using Framework.Security;
//using System.Threading.Tasks;
//using Volo.Abp.Authorization.Permissions;
//using Volo.Abp.Data;
//using Volo.Abp.DependencyInjection;
//using Volo.Abp.MultiTenancy;
//using Volo.Abp.PermissionManagement;

//namespace AuthServer
//{
//    [Dependency(ReplaceServices = true)]
//    [ExposeServices(typeof(PermissionDataSeedContributor))]
//    public class AuthServerPermissionDataSeedContributor : PermissionDataSeedContributor, ITransientDependency
//    {
//        private IPermissionClient PermissionClient { get; }
//        public AuthServerPermissionDataSeedContributor(
//            IPermissionDefinitionManager permissionDefinitionManager,
//            IPermissionDataSeeder permissionDataSeeder,
//            ICurrentTenant currentTenant) : base(permissionDefinitionManager, permissionDataSeeder, currentTenant)
//        {
//        }

//        public override async Task SeedAsync(DataSeedContext context)
//        {
//            await PermissionClient.SyncDefinitions("AuthServer","PAYLOAD","signature");
//        }
//    }
//}
