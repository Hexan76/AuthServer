using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyAuthServer.Data;
using Volo.Abp.DependencyInjection;

namespace MyAuthServer.EntityFrameworkCore;

public class EntityFrameworkCoreMyAuthServerDbSchemaMigrator
    : IMyAuthServerDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreMyAuthServerDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the MyAuthServerDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<MyAuthServerDbContext>()
            .Database
            .MigrateAsync();
    }
}
