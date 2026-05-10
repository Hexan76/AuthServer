using PermissionService.Localization;
using Volo.Abp.Application.Services;

namespace PermissionService;

/* Inherit your application services from this class.
 */
public abstract class PermissionServiceAppService : ApplicationService
{
    protected PermissionServiceAppService()
    {
        LocalizationResource = typeof(PermissionServiceResource);
    }
}
