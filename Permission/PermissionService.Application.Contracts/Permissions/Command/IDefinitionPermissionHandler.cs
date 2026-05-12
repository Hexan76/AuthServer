using Framework.BuildingBlock.Application.Contracts;

namespace PermissionService.Permissions;

public interface IDefinitionPermissionHandler : IFrameworkRequestHandler<DefinitionPermissionsCreate, BaseResponse>
{
}
