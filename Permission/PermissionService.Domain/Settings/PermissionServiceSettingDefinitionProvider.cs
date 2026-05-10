using Volo.Abp.Settings;

namespace PermissionService.Settings;

public class PermissionServiceSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(PermissionServiceSettings.MySetting1));
    }
}
