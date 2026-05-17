using AuthServer.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace AuthServer.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class AuthServerController : AbpControllerBase
{
    protected AuthServerController()
    {
        LocalizationResource = typeof(AuthServerResource);
    }
}
