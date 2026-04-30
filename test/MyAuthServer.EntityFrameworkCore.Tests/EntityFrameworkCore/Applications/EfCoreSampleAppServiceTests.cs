using MyAuthServer.Samples;
using Xunit;

namespace MyAuthServer.EntityFrameworkCore.Applications;

[Collection(MyAuthServerTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<MyAuthServerEntityFrameworkCoreTestModule>
{

}
