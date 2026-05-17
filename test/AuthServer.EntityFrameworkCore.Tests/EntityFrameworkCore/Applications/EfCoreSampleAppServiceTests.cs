using AuthServer.Samples;
using Xunit;

namespace AuthServer.EntityFrameworkCore.Applications;

[Collection(AuthServerTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<AuthServerEntityFrameworkCoreTestModule>
{

}
