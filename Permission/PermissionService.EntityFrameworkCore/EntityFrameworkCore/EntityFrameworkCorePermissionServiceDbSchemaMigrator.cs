using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PermissionService.Data;
using Volo.Abp.DependencyInjection;

namespace PermissionService.EntityFrameworkCore;

public class EntityFrameworkCorePermissionServiceDbSchemaMigrator
    : IPermissionServiceDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCorePermissionServiceDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the PermissionServiceDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<PermissionServiceDbContext>()
            .Database
            .MigrateAsync();
    }
}
