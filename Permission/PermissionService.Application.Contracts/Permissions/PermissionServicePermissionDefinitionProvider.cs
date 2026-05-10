using PermissionService.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace PermissionService.Permissions;

public class PermissionServicePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(PermissionServicePermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(PermissionServicePermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<PermissionServiceResource>(name);
    }
}
