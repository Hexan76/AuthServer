using System.Threading.Tasks;

namespace MyAuthServer.Data;

public interface IMyAuthServerDbSchemaMigrator
{
    Task MigrateAsync();
}
