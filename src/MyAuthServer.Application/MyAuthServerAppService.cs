using MyAuthServer.Localization;
using Volo.Abp.Application.Services;

namespace MyAuthServer;

/* Inherit your application services from this class.
 */
public abstract class MyAuthServerAppService : ApplicationService
{
    protected MyAuthServerAppService()
    {
        LocalizationResource = typeof(MyAuthServerResource);
    }
}
