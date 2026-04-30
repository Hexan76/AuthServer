using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace MyAuthServer.Pages;

[Collection(MyAuthServerTestConsts.CollectionDefinitionName)]
public class Index_Tests : MyAuthServerWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}
