using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AuthServer.Data;

/* This is used if database provider does't define
 * IAuthServerDbSchemaMigrator implementation.
 */
public class NullAuthServerDbSchemaMigrator : IAuthServerDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
