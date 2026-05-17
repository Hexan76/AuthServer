using Microsoft.AspNetCore.Builder;
using AuthServer;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("AuthServer.Web.csproj"); 
await builder.RunAbpModuleAsync<AuthServerWebTestModule>(applicationName: "AuthServer.Web");

public partial class Program
{
}
