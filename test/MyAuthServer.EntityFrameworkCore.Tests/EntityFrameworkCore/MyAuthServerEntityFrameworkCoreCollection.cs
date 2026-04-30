using Xunit;

namespace MyAuthServer.EntityFrameworkCore;

[CollectionDefinition(MyAuthServerTestConsts.CollectionDefinitionName)]
public class MyAuthServerEntityFrameworkCoreCollection : ICollectionFixture<MyAuthServerEntityFrameworkCoreFixture>
{

}
