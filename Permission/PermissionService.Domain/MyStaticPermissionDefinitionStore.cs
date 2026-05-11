using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.StaticDefinitions;

namespace PermissionService
{
    [Dependency(ReplaceServices = true)]
    public class MyStaticPermissionDefinitionStore : StaticPermissionDefinitionStore
    {
        public MyStaticPermissionDefinitionStore(IServiceProvider serviceProvider, IOptions<AbpPermissionOptions> options, IStaticDefinitionCache<PermissionGroupDefinition, (Dictionary<string, PermissionGroupDefinition>, List<PermissionDefinition>)> groupCache, IStaticDefinitionCache<PermissionDefinition, Dictionary<string, PermissionDefinition>> definitionCache) : base(serviceProvider, options, groupCache, definitionCache)
        {
        }

        
    }
}
