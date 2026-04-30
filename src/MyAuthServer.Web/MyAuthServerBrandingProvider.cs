using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using MyAuthServer.Localization;

namespace MyAuthServer.Web;

[Dependency(ReplaceServices = true)]
public class MyAuthServerBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<MyAuthServerResource> _localizer;

    public MyAuthServerBrandingProvider(IStringLocalizer<MyAuthServerResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
