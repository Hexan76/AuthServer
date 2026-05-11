using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
//using Microsoft.OpenApi.Models;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
//using OpenIddict.Validation.AspNetCore;
using PermissionService.EntityFrameworkCore;
using PermissionService.Localization;
using PermissionService.MultiTenancy;
using System;
using System.Collections.Generic;
using System.IO;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Security.Claims;
using Volo.Abp.Swashbuckle;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

namespace PermissionService.Web;

[DependsOn(
    typeof(PermissionServiceHttpApiModule),
    typeof(PermissionServiceApplicationModule),
    typeof(PermissionServiceEntityFrameworkCoreModule),
    typeof(AbpAutofacModule),
    //typeof(AbpSwashbuckleModule),
    typeof(AbpAspNetCoreSerilogModule)
)]
public class PermissionServiceWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(PermissionServiceResource),
                typeof(PermissionServiceDomainModule).Assembly,
                typeof(PermissionServiceDomainSharedModule).Assembly,
                typeof(PermissionServiceApplicationModule).Assembly,
                typeof(PermissionServiceApplicationContractsModule).Assembly,
                typeof(PermissionServiceWebModule).Assembly
            );
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (!configuration.GetValue<bool>("App:DisablePII"))
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        }

        // =========================
        // AUTH CONFIG (IMPORTANT FIX)
        // =========================
        context.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        });

        context.Services.AddOpenIddict()
            .AddValidation(options =>
            {
                options.SetIssuer("https://localhost:44310/");
                options.AddAudiences("MyAuthServer");

                options.UseSystemNetHttp();
                options.UseAspNetCore();
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

        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });

        Configure<PermissionManagementOptions>(options =>
        {
            options.IsDynamicPermissionStoreEnabled = true;
        });

        ConfigureSwagger(context.Services);
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureAutoApiControllers();
        ConfigureUrls(configuration);
    }

    private void ConfigureSwagger(IServiceCollection services)
    {
        services.AddAbpSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "PermissionService API",
                Version = "v1"
            });

            options.DocInclusionPredicate((docName, desc) => true);
            options.CustomSchemaIds(type => type.FullName);
        });
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
                            { "MyAuthServer", "Access API" }
                        }
                    }
                }
            });
            c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("oauth2", document)] = []
            });
        });
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
        });
    }

    private void ConfigureVirtualFileSystem(IWebHostEnvironment env)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<PermissionServiceWebModule>();

            if (env.IsDevelopment())
            {
                options.FileSets.ReplaceEmbeddedByPhysical<PermissionServiceDomainSharedModule>(
                    Path.Combine(env.ContentRootPath, "..\\PermissionService.Domain.Shared"));

                options.FileSets.ReplaceEmbeddedByPhysical<PermissionServiceDomainModule>(
                    Path.Combine(env.ContentRootPath, "..\\PermissionService.Domain"));

                options.FileSets.ReplaceEmbeddedByPhysical<PermissionServiceApplicationModule>(
                    Path.Combine(env.ContentRootPath, "..\\PermissionService.Application"));

                options.FileSets.ReplaceEmbeddedByPhysical<PermissionServiceApplicationContractsModule>(
                    Path.Combine(env.ContentRootPath, "..\\PermissionService.Application.Contracts"));

                options.FileSets.ReplaceEmbeddedByPhysical<PermissionServiceHttpApiModule>(
                    Path.Combine(env.ContentRootPath, "..\\..\\src\\PermissionService.HttpApi"));
            }
        });
    }

    private void ConfigureAutoApiControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(PermissionServiceApplicationModule).Assembly);
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

        app.UseRouting();

        app.UseCors("SwaggerCors");
        app.UseAuthentication();

        // 🔥 IMPORTANT: TOKEN VALIDATION ENABLED
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            // app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "PermissionService API");

            options.OAuthClientId("swagger");
            options.OAuthClientSecret(null);
            options.OAuthUsePkce();

            options.OAuthScopes("openid", "profile", "MyAuthServer");
        });

        app.UseConfiguredEndpoints();
    }
}