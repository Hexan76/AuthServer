using Framework.BuildingBlock.HttpApi;
using NSwag;
using System;
using System.Collections.Generic;

namespace PermissionService.Web;

public static class NSwagSections
{
    public static SwaggerModuleOptions[] GetSections()
    {
        return
        [
            new SwaggerModuleOptions
            {
                DocumentName = "PermissionService-v1",
                Title = "PermissionService API",
                Version = "v1",
                ApiVersion = 1,
                ExcludeNonFastEndpoints = true,

                EndpointFilter = ep =>
                    ep.EndpointTags?.Contains("Permissions") == true,

                SecurityDefinitions = GetSecurityScheme()
            },

            new SwaggerModuleOptions
            {
                DocumentName = "PermissionService-v2",
                Title = "PermissionService API",
                Version = "v2",
                ApiVersion = 2,
                ExcludeNonFastEndpoints = true,

                EndpointFilter = ep =>
                    ep.EndpointTags?.Contains("Permissions") == true,

                SecurityDefinitions = GetSecurityScheme()
            }
        ];
    }

    private static Dictionary<string, OpenApiSecurityScheme> GetSecurityScheme()
    {
        return new()
        {
            ["OpenIddict"] = new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.OAuth2,
                Flow = OpenApiOAuth2Flow.AccessCode,
                Description ="OAuth2 Authorization Code Flow",
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl =
                                "https://localhost:44310/connect/authorize",

                        TokenUrl =
                                "https://localhost:44310/connect/token",

                        Scopes = new Dictionary<string, string>
                            {
                                { "openid", "OpenId" },
                                { "profile", "Profile" },
                                { "offline_access", "Refresh token access" },
                                { "MyAuthServer", "Main API" }
                            }
                    },
                    AuthorizationCode =
                        new OpenApiOAuthFlow
                        {
                            AuthorizationUrl =
                                "https://localhost:44310/connect/authorize",

                            TokenUrl =
                                "https://localhost:44310/connect/token",

                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "OpenId" },
                                { "profile", "Profile" },
                                { "offline_access", "Refresh token access" },
                                { "MyAuthServer", "Main API" }
                            }
                        },
                },
            }
        };
    }
}