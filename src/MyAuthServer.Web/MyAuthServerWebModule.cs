using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using MyAuthServer.EntityFrameworkCore;
using MyAuthServer.Localization;
using MyAuthServer.MultiTenancy;
using MyAuthServer.Web.HealthChecks;
using OpenIddict.Server.AspNetCore;

//using MyAuthServer.Web.Menus;
using System.IO;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
//using OpenIddict.Server.AspNetCore;
//using OpenIddict.Validation.AspNetCore;
//using Microsoft.AspNetCore.Extensions.DependencyInjection;
//using Volo.Abp.Account.Web;
//using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
//using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
//using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Security.Claims;
using Volo.Abp.Studio;
using Volo.Abp.Studio.Client.AspNetCore;
using Volo.Abp.Swashbuckle;
using Volo.Abp.UI.Navigation;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

namespace MyAuthServer.Web;

[DependsOn(
    typeof(MyAuthServerHttpApiModule),
    typeof(MyAuthServerApplicationModule),
    typeof(MyAuthServerEntityFrameworkCoreModule),
    typeof(AbpAutofacModule),
    typeof(AbpStudioClientAspNetCoreModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(AbpSwashbuckleModule),

    typeof(AbpAccountHttpApiModule),
    typeof(AbpIdentityAspNetCoreModule),
    typeof(AbpAccountApplicationModule),

    typeof(AbpAspNetCoreSerilogModule)
)]
public class MyAuthServerWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(MyAuthServerResource),
                typeof(MyAuthServerDomainModule).Assembly,
                typeof(MyAuthServerDomainSharedModule).Assembly,
                typeof(MyAuthServerApplicationModule).Assembly,
                typeof(MyAuthServerApplicationContractsModule).Assembly,
                typeof(MyAuthServerWebModule).Assembly
            );
        });

        context.Services.AddAuthentication(options =>
        {
            options.AddScheme("Github", Gitoptions =>
            {
                Gitoptions.DisplayName = "Github2";
                Gitoptions.HandlerType = typeof(OpenIddictServerAspNetCoreHandler);
            });
        });

        // Add OpenIddict for client configuration
        context.Services.AddOpenIddict()
            .AddClient(options =>
            {
                // Add GitHub authentication using values from the configuration
                options.UseWebProviders()
                    .AddGitHub(githubOptions =>
                    {
                        githubOptions.SetClientId(configuration["Authentication:Github:ClientId"])  // Get from appsettings
                                     .SetClientSecret(configuration["Authentication:Github:ClientSecret"])  // Get from appsettings
                                     .SetRedirectUri("https://localhost:44310/callback/login/github");  // Redirect URI
                    })
                    // Add Google authentication using values from the configuration
                    .AddGoogle(googleOptions =>
                    {
                        googleOptions.SetClientId(configuration["Authentication:Google:ClientId"])  // Get from appsettings
                                     .SetClientSecret(configuration["Authentication:Google:ClientSecret"])  // Get from appsettings
                                     .SetRedirectUri("https://localhost:44310/callback/login/google");  // Redirect URI
                    });
            });


        PreConfigure<OpenIddictBuilder>(builder =>
            {
                builder.AddServer(options =>
                {
                    // Endpoints (standard OIDC)
                    options.SetAuthorizationEndpointUris("/connect/authorize");
                    options.SetTokenEndpointUris("/connect/token");
                    options.SetUserInfoEndpointUris("/connect/userinfo");

                    // === OAuth Flows (ONLY what Node.js needs) ===
                    options.AllowAuthorizationCodeFlow();   // SPA + web login
                    options.AllowRefreshTokenFlow();        // session persistence
                    options.AllowClientCredentialsFlow();   // backend services

                    // 🔐 REQUIRED for SPA / Node.js frontend
                    options.RequireProofKeyForCodeExchange(); // PKCE


                    options.AddDevelopmentEncryptionCertificate()
                           .AddDevelopmentSigningCertificate();
                    // Standard OIDC scopes
                    options.RegisterScopes(
                        "openid",
                        "profile",
                        "email",
                        "roles",
                        "MyAuthServer"
                    );


                    // ASP.NET integration
                    options.UseAspNetCore()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableTokenEndpointPassthrough()
                        .EnableUserInfoEndpointPassthrough();
                });

                builder.AddValidation(options =>
                {
                    options.AddAudiences("MyAuthServer");
                    options.UseLocalServer();
                    options.UseAspNetCore();
                });
            });

        if (!hostingEnvironment.IsDevelopment())
        {
            PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
            {
                options.AddDevelopmentEncryptionAndSigningCertificate = false;
            });
        }
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        if (!configuration.GetValue<bool>("App:DisablePII"))
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.LogCompleteSecurityArtifact = true;
        }

        if (!configuration.GetValue<bool>("AuthServer:RequireHttpsMetadata"))
        {
            //    Configure<OpenIddictServerAspNetCoreOptions>(options =>
            //    {
            //        options.DisableTransportSecurityRequirement = true;
            //    });

            Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
                options.KnownIPNetworks.Clear();
                options.KnownProxies.Clear();
            });
        }

        if (hostingEnvironment.IsDevelopment())
        {
            context.Services.AddRazorPages()
                .AddRazorRuntimeCompilation();
        }

        ConfigureStudio(hostingEnvironment);
        ConfigureBundles(hostingEnvironment);
        ConfigureUrls(configuration);
        ConfigureHealthChecks(context);
        ConfigureAuthentication(context);
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureNavigationServices();
        ConfigureAutoApiControllers();
        ConfigureSwaggerServices(context.Services);

        Configure<PermissionManagementOptions>(options =>
        {
            options.IsDynamicPermissionStoreEnabled = true;
        });
    }


    private void ConfigureHealthChecks(ServiceConfigurationContext context)
    {
        context.Services.AddMyAuthServerHealthChecks();
    }

    private void ConfigureStudio(IHostEnvironment hostingEnvironment)
    {
        if (hostingEnvironment.IsProduction())
        {
            Configure<AbpStudioClientOptions>(options =>
            {
                options.IsLinkEnabled = false;
            });
        }
    }

    private void ConfigureBundles(IHostEnvironment hostingEnvironment)
    {

    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
        });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        //context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });
    }

    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<MyAuthServerWebModule>();

            if (hostingEnvironment.IsDevelopment())
            {
                options.FileSets.ReplaceEmbeddedByPhysical<MyAuthServerDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}MyAuthServer.Domain.Shared", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<MyAuthServerDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}MyAuthServer.Domain", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<MyAuthServerApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}MyAuthServer.Application.Contracts", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<MyAuthServerApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}MyAuthServer.Application", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<MyAuthServerHttpApiModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}MyAuthServer.HttpApi", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<MyAuthServerWebModule>(hostingEnvironment.ContentRootPath);
            }
        });
    }

    private void ConfigureNavigationServices()
    {
        //Configure<AbpNavigationOptions>(options =>
        //{
        //    options.MenuContributors.Add(new MyAuthServerMenuContributor());
        //});

        //Configure<AbpToolbarOptions>(options =>
        //{
        //    options.Contributors.Add(new MyAuthServerToolbarContributor());
        //});
    }

    private void ConfigureAutoApiControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(MyAuthServerApplicationModule).Assembly);
        });
    }

    private void ConfigureSwaggerServices(IServiceCollection services)
    {
        services.AddAbpSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAuthServer API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            }
        );
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        app.UseForwardedHeaders();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            //app.UseErrorPage();
            app.UseHsts();
        }

        app.UseCorrelationId();
        app.UseRouting();
        app.MapAbpStaticAssets();
        app.UseAbpStudioLink();
        app.UseAbpSecurityHeaders();
        app.UseAuthentication();
        //app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "MyAuthServer API");
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}
