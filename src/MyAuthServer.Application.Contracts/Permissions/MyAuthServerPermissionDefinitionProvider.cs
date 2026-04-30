using MyAuthServer.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace MyAuthServer.Permissions;

public class MyAuthServerPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(MyAuthServerPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(MyAuthServerPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<MyAuthServerResource>(name);
    }
}
