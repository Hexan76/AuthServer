using FastEndpoints;
using Framework.BuildingBlock.Application.Contracts;
using Framework.BuildingBlock.HttpApi;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text.Json;

namespace PermissionService.Permissions;

public class PermissionSync : BaseEndpoint<DefinitionPermissionsCreate, BaseResponse>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("api/permission-management/sync");
        Tags(["Permissions"]);
        Version(1);
        Options(c => c.WithTags(["Permissions"]));
        //TODO: NSwag Documention
        Summary(s =>
        {
            s.Description = "Synchronize permissions with the provided list. This endpoint will add new permissions, update existing ones, and remove any permissions that are not included in the request.";
            s.Summary = "Synchronize permissions with the provided list.";
            s.ExampleRequest = new DefinitionPermissionsCreate
            {
                ServiceName = "ServiceName",
                Payload = JsonSerializer.Serialize(new List<FrameworkPermissionModel> { }),
                Signature = "Signature for the payload"
            };
        });
        //TODO: if use OpenApi need to use use this method to add description for the endpoint, if not use OpenApi, we can use Summary method to add description for the endpoint, need to test it in the future.
        //Description(s =>
        //{
        //    s.WithSummary("Synchronize permissions with the provided list.");
        //    s.WithDescription("Synchronize permissions with the provided list. This endpoint will add new permissions, update existing ones, and remove any permissions that are not included in the request.");

        //}); 

        AllowAnonymous();
    }
}
