using System.Threading.Tasks;

namespace AuthServer.Data;

public interface IAuthServerDbSchemaMigrator
{
    Task MigrateAsync();
}
