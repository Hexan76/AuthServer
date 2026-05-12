using Framework.BuildingBlock.Application.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace PermissionService.Permissions;

public interface IPermissionSyncEngine
{
    Task SyncAsync(PermissionSyncSnapshot snapshot, CancellationToken ct = default);
}
