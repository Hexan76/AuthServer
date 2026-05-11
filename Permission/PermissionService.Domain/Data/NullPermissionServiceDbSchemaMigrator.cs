using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace PermissionService.Data;

/* This is used if database provider does't define
 * IPermissionServiceDbSchemaMigrator implementation.
 */
public class NullPermissionServiceDbSchemaMigrator : IPermissionServiceDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
