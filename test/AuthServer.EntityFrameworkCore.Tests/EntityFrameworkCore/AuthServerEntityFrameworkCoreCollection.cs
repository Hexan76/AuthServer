using Xunit;

namespace AuthServer.EntityFrameworkCore;

[CollectionDefinition(AuthServerTestConsts.CollectionDefinitionName)]
public class AuthServerEntityFrameworkCoreCollection : ICollectionFixture<AuthServerEntityFrameworkCoreFixture>
{

}
