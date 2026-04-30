using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MyAuthServer.Data;

/* This is used if database provider does't define
 * IMyAuthServerDbSchemaMigrator implementation.
 */
public class NullMyAuthServerDbSchemaMigrator : IMyAuthServerDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
