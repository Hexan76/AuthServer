using Framework.BuildingBlock.Application.Contracts;

namespace PermissionService.Permissions;

public class DefinitionPermissionsCreate : IFrameworkRequest<BaseResponse>
{
    public string ServiceName { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public string Signature { get; set; } = default!;
}
