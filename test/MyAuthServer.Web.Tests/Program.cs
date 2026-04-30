using Microsoft.AspNetCore.Builder;
using MyAuthServer;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("MyAuthServer.Web.csproj"); 
await builder.RunAbpModuleAsync<MyAuthServerWebTestModule>(applicationName: "MyAuthServer.Web");

public partial class Program
{
}
