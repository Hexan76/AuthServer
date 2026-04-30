using MyAuthServer.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace MyAuthServer.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class MyAuthServerController : AbpControllerBase
{
    protected MyAuthServerController()
    {
        LocalizationResource = typeof(MyAuthServerResource);
    }
}
