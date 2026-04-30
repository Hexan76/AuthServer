using Volo.Abp.Settings;

namespace MyAuthServer.Settings;

public class MyAuthServerSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(MyAuthServerSettings.MySetting1));
    }
}
