using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Modularity;

namespace MyAuthServer.DbMigrator
{
    [Dependency(ReplaceServices = true)]
    public class MyDataSeeder : DataSeeder, ITransientDependency
    {
        public MyDataSeeder(IOptions<AbpDataSeedOptions> options, IServiceScopeFactory serviceScopeFactory) : base(options, serviceScopeFactory)
        {
        }
        public override Task SeedAsync(DataSeedContext context)
        {
            return base.SeedAsync(context);
        }
    }
}
