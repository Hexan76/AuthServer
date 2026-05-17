using AuthServer.Localization;
using Volo.Abp.Application.Services;

namespace AuthServer;

/* Inherit your application services from this class.
 */
public abstract class AuthServerAppService : ApplicationService
{
    protected AuthServerAppService()
    {
        LocalizationResource = typeof(AuthServerResource);
    }
}
