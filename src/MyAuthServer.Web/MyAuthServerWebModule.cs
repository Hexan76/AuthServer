using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
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
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using System;
using System.Collections.Generic;



//using MyAuthServer.Web.Menus;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Libs;
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
//using Volo.Abp.UI.Navigation;
//using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

namespace MyAuthServer.Web;

[DependsOn(
    typeof(MyAuthServerHttpApiModule),
    typeof(MyAuthServerApplicationModule),
    typeof(MyAuthServerEntityFrameworkCoreModule),
    typeof(AbpAutofacModule),
    typeof(AbpStudioClientAspNetCoreModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    //typeof(AbpSwashbuckleModule),

    typeof(AbpOpenIddictAspNetCoreModule),
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
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.LoginPath = "/account2/login";
        })
        .AddGitHub(gitOptions =>
        {
            gitOptions.ClientId = configuration["Authentication:Github:ClientId"];
            gitOptions.ClientSecret = configuration["Authentication:Github:ClientSecret"];
            gitOptions.CallbackPath = "/callback/login/github";
        })
        .AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
            googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
            googleOptions.CallbackPath = "/callback/login/github";

            googleOptions.Events.OnRedirectToAuthorizationEndpoint = context =>
            {
                context.Response.Redirect(context.RedirectUri + "&prompt=consent");
                return Task.CompletedTask;
            };
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
                                     .SetRedirectUri("https://localhost:44310/Account2/Login?handler=ExternalLoginCallback");  // Redirect URI
                    });
            });

        PreConfigure<OpenIddictBuilder>(builder =>
            {
                builder.AddServer(options =>
                {
                    options.SetAuthorizationEndpointUris("/connect/authorize");
                    options.SetTokenEndpointUris("/connect/token");
                    options.SetUserInfoEndpointUris("/connect/userinfo");
                    options.SetAccessTokenLifetime(TimeSpan.FromMinutes(1));
                    options
                        .AllowAuthorizationCodeFlow()
                        .AllowHybridFlow()
                        .AllowImplicitFlow()
                        .AllowPasswordFlow()
                        .AllowClientCredentialsFlow()
                        .AllowRefreshTokenFlow()
                        .AllowDeviceAuthorizationFlow()
                        .AllowNoneFlow()
                        .AllowTokenExchangeFlow();

                    options.RequireProofKeyForCodeExchange(); 


                    options.AddDevelopmentEncryptionCertificate()
                           .AddDevelopmentSigningCertificate();
                    // Standard OIDC scopes
                    options.RegisterScopes(new[]
                           {
                                OpenIddictConstants.Scopes.OpenId,
                                OpenIddictConstants.Scopes.Email,
                                OpenIddictConstants.Scopes.Profile,
                                OpenIddictConstants.Scopes.Phone,
                                OpenIddictConstants.Scopes.Roles,
                                OpenIddictConstants.Scopes.Address,
                                OpenIddictConstants.Scopes.OfflineAccess,
                                "MyAuthServer"
                           });


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

        context.Services.AddCors(options =>
        {
            options.AddPolicy("SwaggerCors", policy =>
            {
                policy
                    .WithOrigins("https://localhost:44310") // AuthServer
                    .WithOrigins("https://localhost:44366") // PermissionService Swagger
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
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


        Configure<AbpMvcLibsOptions>(options =>
        {
            options.CheckLibs = false;
        });

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
            options.IsDynamicPermissionStoreEnabled = false;
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
        //Configure<AppUrlOptions>(options =>
        //{
        //    options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
        //});
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
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
                //options.FileSets.ReplaceEmbeddedByPhysical<MyAuthServerWebModule>(hostingEnvironment.ContentRootPath);
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
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri("https://localhost:44310/connect/authorize"),
                        TokenUrl = new Uri("https://localhost:44310/connect/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "OpenId" },
                            { "profile", "Profile" },
                            { "offline_access", "Refresh token access" },
                            { "MyAuthServer", "Main API" }                        }
                    }
                }
            });
            c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("oauth2", document)] = []
            });
        });
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
        app.UseCors("SwaggerCors");

        app.MapAbpStaticAssets();
        app.UseAbpStudioLink();
        app.UseAbpSecurityHeaders();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "MyAuthServer API");
            options.OAuthClientId("swagger");
            options.OAuthClientSecret(null);
            options.OAuthScopes("MyAuthServer");
            options.OAuthUsePkce();

        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}
