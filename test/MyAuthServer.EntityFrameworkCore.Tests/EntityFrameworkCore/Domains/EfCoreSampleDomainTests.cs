using MyAuthServer.Samples;
using Xunit;

namespace MyAuthServer.EntityFrameworkCore.Domains;

[Collection(MyAuthServerTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<MyAuthServerEntityFrameworkCoreTestModule>
{

}
