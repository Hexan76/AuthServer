using Framework.BuildingBlock.Domain.Shared;
using PermissionService.Localization;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace PermissionService;

[DependsOn(
    typeof(AbpAuditLoggingDomainSharedModule),
    typeof(AbpBackgroundJobsDomainSharedModule),
    typeof(BuildingBlockDomainSharedModule),
    typeof(AbpPermissionManagementDomainSharedModule)
    )]
public class PermissionServiceDomainSharedModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PermissionServiceGlobalFeatureConfigurator.Configure();
        PermissionServiceModuleExtensionConfigurator.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<PermissionServiceDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<PermissionServiceResource>("en")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Localization/PermissionService");

            options.DefaultResourceType = typeof(PermissionServiceResource);

            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("ar", "ar", "Arabic"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "Chinese (Simplified)"));
            options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "Chinese (Traditional)"));
            options.Languages.Add(new LanguageInfo("cs", "cs", "Czech"));
            options.Languages.Add(new LanguageInfo("en-GB", "en-GB", "English (United Kingdom)"));
            options.Languages.Add(new LanguageInfo("fi", "fi", "Finnish"));
            options.Languages.Add(new LanguageInfo("fr", "fr", "French"));
            options.Languages.Add(new LanguageInfo("de-DE", "de-DE", "German (Germany)"));
            options.Languages.Add(new LanguageInfo("hi", "hi", "Hindi "));
            options.Languages.Add(new LanguageInfo("hu", "hu", "Hungarian"));
            options.Languages.Add(new LanguageInfo("is", "is", "Icelandic"));
            options.Languages.Add(new LanguageInfo("it", "it", "Italian"));
            options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Portuguese (Brazil)"));
            options.Languages.Add(new LanguageInfo("ro-RO", "ro-RO", "Romanian (Romania)"));
            options.Languages.Add(new LanguageInfo("ru", "ru", "Russian"));
            options.Languages.Add(new LanguageInfo("sk", "sk", "Slovak"));
            options.Languages.Add(new LanguageInfo("es", "es", "Spanish"));
            options.Languages.Add(new LanguageInfo("sv", "sv", "Swedish"));
            options.Languages.Add(new LanguageInfo("tr", "tr", "Turkish"));

        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("PermissionService", typeof(PermissionServiceResource));
        });
    }
}
