using FastEndpoints;
using Framework.BuildingBlock.Application.Contracts;
using Framework.BuildingBlock.HttpApi;
using Microsoft.AspNetCore.Http;

namespace PermissionService.Permissions;

public class PermissionSync : BaseEndpoint<DefinitionPermissionsCreate, BaseResponse>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("api/permission-management/sync");
        Tags(["Permission Management"]);
        Options(c => c.WithTags(["Permission Management"]));
        AllowAnonymous();
    }
}
