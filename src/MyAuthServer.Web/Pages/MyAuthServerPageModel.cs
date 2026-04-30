using MyAuthServer.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace MyAuthServer.Web.Pages;

public abstract class MyAuthServerPageModel : AbpPageModel
{
    protected MyAuthServerPageModel()
    {
        LocalizationResourceType = typeof(MyAuthServerResource);
    }
}
