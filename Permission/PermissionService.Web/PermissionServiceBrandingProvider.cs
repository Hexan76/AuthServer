using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using PermissionService.Localization;

namespace PermissionService.Web;

[Dependency(ReplaceServices = true)]
public class PermissionServiceBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<PermissionServiceResource> _localizer;

    public PermissionServiceBrandingProvider(IStringLocalizer<PermissionServiceResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
