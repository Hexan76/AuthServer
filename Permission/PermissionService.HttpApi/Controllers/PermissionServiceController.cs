using PermissionService.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace PermissionService.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class PermissionServiceController : AbpControllerBase
{
    protected PermissionServiceController()
    {
        LocalizationResource = typeof(PermissionServiceResource);
    }
}
