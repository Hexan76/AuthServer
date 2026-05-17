using AuthServer.Samples;
using Xunit;

namespace AuthServer.EntityFrameworkCore.Domains;

[Collection(AuthServerTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<AuthServerEntityFrameworkCoreTestModule>
{

}
