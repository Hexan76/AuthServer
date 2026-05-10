using System.Threading.Tasks;

namespace PermissionService.Data;

public interface IPermissionServiceDbSchemaMigrator
{
    Task MigrateAsync();
}
