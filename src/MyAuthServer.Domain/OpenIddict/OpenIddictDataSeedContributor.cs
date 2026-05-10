using Microsoft.Extensions.Configuration;
using OpenIddict.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.OpenIddict.Scopes;
using Volo.Abp.Uow;

namespace MyAuthServer.OpenIddict;

/* Creates initial data that is needed to property run the application
 * and make client-to-server communication possible.
 */
public class OpenIddictDataSeedContributor : OpenIddictDataSeedContributorBase, IDataSeedContributor, ITransientDependency
{
    public OpenIddictDataSeedContributor(
        IConfiguration configuration,
        IOpenIddictApplicationRepository openIddictApplicationRepository,
        IAbpApplicationManager applicationManager,
        IOpenIddictScopeRepository openIddictScopeRepository,
        IOpenIddictScopeManager scopeManager)
        : base(configuration, openIddictApplicationRepository, applicationManager, openIddictScopeRepository, scopeManager)
    {
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        await CreateScopesAsync();
        await CreateApplicationsAsync();
    }

    private async Task CreateScopesAsync()
    {
        await CreateScopesAsync(new OpenIddictScopeDescriptor
        {
            Name = "MyAuthServer",
            DisplayName = "MyAuthServer API",
            Resources = { "MyAuthServer" }
        });
    }

    private async Task CreateApplicationsAsync()
    {
        var commonScopes = new List<string>
    {
        "openid",
        OpenIddictConstants.Permissions.Scopes.Address,
        OpenIddictConstants.Permissions.Scopes.Profile,
        OpenIddictConstants.Permissions.Scopes.Email,
        OpenIddictConstants.Permissions.Scopes.Roles,
        "MyAuthServer"
    };

        var configurationSection = Configuration.GetSection("OpenIddict:Applications");

        var consoleAndAngularClientId = configurationSection["MyAuthServer_App:ClientId"];

        //if (!consoleAndAngularClientId.IsNullOrWhiteSpace())
        //{
        //    var rootUrl = Configuration.GetSection["App:SelfUrl"]?.TrimEnd('/');

        //    if (rootUrl.IsNullOrWhiteSpace())
        //        throw new AbpException("RootUrl missing for Angular client");

        //    await CreateOrUpdateApplicationAsync(
        //        applicationType: OpenIddictConstants.ApplicationTypes.Web,
        //        name: consoleAndAngularClientId!,
        //        type: OpenIddictConstants.ClientTypes.Public,
        //        consentType: OpenIddictConstants.ConsentTypes.Implicit,
        //        displayName: "Angular Client",
        //        secret: null,
        //        grantTypes: new List<string>
        //        {
        //        OpenIddictConstants.GrantTypes.AuthorizationCode
        //        },
        //        scopes: commonScopes,
        //        redirectUris: new List<string> { $"{rootUrl}/callback" },
        //        postLogoutRedirectUris: new List<string> { rootUrl },
        //        clientUri: rootUrl,
        //        logoUri: "/images/clients/angular.svg"
        //    );
        //}

        await CreateNodeJsSpaClientAsync(commonScopes);
        await CreatePostmanClientAsync(commonScopes);
        await CreatePermissionServiceAsync(commonScopes);

        var swaggerClientId = configurationSection["MyAuthServer_Swagger:ClientId"];

        if (!swaggerClientId.IsNullOrWhiteSpace())
        {
            var swaggerRootUrl = configurationSection["MyAuthServer_Swagger:RootUrl"]?.TrimEnd('/');

            await CreateOrUpdateApplicationAsync(
                applicationType: OpenIddictConstants.ApplicationTypes.Web,
                name: swaggerClientId!,
                type: OpenIddictConstants.ClientTypes.Public,
                consentType: OpenIddictConstants.ConsentTypes.External,
                displayName: "Swagger UI",
                secret: null,
                grantTypes: new List<string>
                {
                        OpenIddictConstants.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange // 🔥 PKCE
                },
                scopes: commonScopes,
                redirectUris: new List<string> { $"{swaggerRootUrl}/swagger/oauth2-redirect.html" },
                clientUri: swaggerRootUrl + "/swagger",
                logoUri: "/images/clients/swagger.svg"
            );
        }
    }
    private async Task CreateNodeJsSpaClientAsync(List<string> commonScopes)
    {
        await CreateOrUpdateApplicationAsync(
            applicationType: OpenIddictConstants.ApplicationTypes.Web,
            name: "react-spa",
            type: OpenIddictConstants.ClientTypes.Confidential,
            consentType: OpenIddictConstants.ConsentTypes.Implicit,

            displayName: "React SPA",

            secret: "react-spa-Secret",

            grantTypes: new List<string>
            {
                OpenIddictConstants.GrantTypes.AuthorizationCode
            },

            scopes: new List<string>
            {
                OpenIddictConstants.Permissions.Scopes.Address,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Roles,
                "MyAuthServer"
            },

            redirectUris: new List<string>
            {
                "http://localhost:3000/callback"
            },

            postLogoutRedirectUris: new List<string>
            {
                "http://localhost:3000"
            }
        );
    }
    private async Task CreatePermissionServiceAsync(List<string> commonScopes)
    {
        await CreateOrUpdateApplicationAsync(
            applicationType: OpenIddictConstants.ApplicationTypes.Web,
            name: "permission",
            type: OpenIddictConstants.ClientTypes.Confidential,
            consentType: OpenIddictConstants.ConsentTypes.Implicit,

            displayName: "Permission Service",
            secret: "permission-secret",

            grantTypes: new List<string>
            {
                OpenIddictConstants.GrantTypes.ClientCredentials
            },

            scopes: new List<string>
            {
                "permissions.read",
                "permissions.write"
            },

            redirectUris: new List<string>
            {
                "https://oauth.pstmn.io/v1/callback"
            },

            postLogoutRedirectUris: new List<string>
            {

            }
        );
    }
    private async Task CreatePostmanClientAsync(List<string> commonScopes)
    {
        await CreateOrUpdateApplicationAsync(
            applicationType: OpenIddictConstants.ApplicationTypes.Native,
            name: "postman",
            type: OpenIddictConstants.ClientTypes.Public,
            consentType: OpenIddictConstants.ConsentTypes.Implicit,

            displayName: "Postman",

            secret: null,

            grantTypes: new List<string>
            {
                OpenIddictConstants.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange // 🔥 PKCE
            },

            scopes: new List<string>
            {
                OpenIddictConstants.Permissions.Scopes.Address,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Roles,
                "MyAuthServer"
            },

            redirectUris: new List<string>
            {
                "https://oauth.pstmn.io/v1/callback"
            },

            postLogoutRedirectUris: new List<string>
            {

            }
        );
    }
}
