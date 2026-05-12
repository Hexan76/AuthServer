using Framework.BuildingBlock.Application.Contracts;
using Framework.BuildingBlock.Permissions;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PermissionService.Permissions;

public class DefinitionPermissionHandler(IPermissionSyncEngine permissionSyncEngine) : PermissionServiceAppService, IDefinitionPermissionHandler
{
    public async Task<MessageContract<BaseResponse>> Handle(
        DefinitionPermissionsCreate request,
        CancellationToken cancellationToken)
    {
        if (!PermissionSignatureHelper.Verify(request.Payload, request.Signature))
            throw new UnauthorizedAccessException();

        var data = JsonSerializer.Deserialize<PermissionSyncSnapshot>(request.Payload);

        await permissionSyncEngine.SyncAsync(data, cancellationToken);

        return new AcceptMessage<BaseResponse>();
    }
}
