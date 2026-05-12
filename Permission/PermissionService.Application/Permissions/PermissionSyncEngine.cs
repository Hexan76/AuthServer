using Framework.BuildingBlock.Application.Contracts;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.PermissionManagement;

namespace PermissionService.Permissions;

public class PermissionSyncEngine(IPermissionGroupDefinitionRecordRepository PermissionGroupRepository,
                                  IPermissionDefinitionRecordRepository PermissionRepository,
                                  IOptions<AbpPermissionOptions> options) : IPermissionSyncEngine ,ITransientDependency
{
    protected AbpPermissionOptions PermissionOptions { get; } = options.Value;

    public async Task SyncAsync(
        PermissionSyncSnapshot snapshot,
        CancellationToken ct = default)
    {
        if (snapshot == null || snapshot.Groups == null || snapshot.Groups.Count == 0)
            return;

        var groupRecords = snapshot.Groups
            .Select(MapGroup)
            .ToList();

        var permissionRecords = snapshot.Groups
            .SelectMany(MapPermissions)
            .ToList();

        await UpdateChangedPermissionGroupsAsync(
            groupRecords,
            ct
        );

        var newOrChangedPermissions = new List<string>();

        await UpdateChangedPermissionsAsync(
            permissionRecords,
            newOrChangedPermissions,
            ct
        );
    }

    private async Task<bool> UpdateChangedPermissionGroupsAsync(
    IEnumerable<PermissionGroupDefinitionRecord> permissionGroupRecords, CancellationToken ct = default)
    {
        var newRecords = new List<PermissionGroupDefinitionRecord>();
        var changedRecords = new List<PermissionGroupDefinitionRecord>();

        var permissionGroupRecordsInDatabase = (await PermissionGroupRepository.GetListAsync(false, ct))
            .ToDictionary(x => x.Name);

        foreach (var permissionGroupRecord in permissionGroupRecords)
        {
            var permissionGroupRecordInDatabase =
                permissionGroupRecordsInDatabase.GetOrDefault(permissionGroupRecord.Name);
            if (permissionGroupRecordInDatabase == null)
            {
                /* New group */
                newRecords.Add(permissionGroupRecord);
                continue;
            }

            if (permissionGroupRecord.HasSameData(permissionGroupRecordInDatabase))
            {
                /* Not changed */
                continue;
            }

            /* Changed */
            permissionGroupRecordInDatabase.Patch(permissionGroupRecord);
            changedRecords.Add(permissionGroupRecordInDatabase);
        }

        /* Deleted */
        var deletedRecords = PermissionOptions.DeletedPermissionGroups.Any()
            ? permissionGroupRecordsInDatabase.Values
                .Where(x => PermissionOptions.DeletedPermissionGroups.Contains(x.Name))
                .ToArray()
            : Array.Empty<PermissionGroupDefinitionRecord>();

        if (newRecords.Any())
        {
            await PermissionGroupRepository.InsertManyAsync(newRecords, false, ct);
        }

        if (changedRecords.Any())
        {
            await PermissionGroupRepository.UpdateManyAsync(changedRecords, false, ct);
        }

        if (deletedRecords.Any())
        {
            await PermissionGroupRepository.DeleteManyAsync(deletedRecords, false, ct);
        }

        return newRecords.Any() || changedRecords.Any() || deletedRecords.Any();
    }

    private async Task<bool> UpdateChangedPermissionsAsync(IEnumerable<PermissionDefinitionRecord> permissionRecords,
        List<string> newOrChangedPermissions,
        CancellationToken ct = default)
    {
        var newRecords = new List<PermissionDefinitionRecord>();
        var changedRecords = new List<PermissionDefinitionRecord>();

        var permissionRecordsInDatabase = (await PermissionRepository.GetListAsync(false, ct))
            .ToDictionary(x => x.Name);

        foreach (var permissionRecord in permissionRecords)
        {
            var permissionRecordInDatabase = permissionRecordsInDatabase.GetOrDefault(permissionRecord.Name);
            if (permissionRecordInDatabase == null)
            {
                /* New permission */
                newRecords.Add(permissionRecord);
                continue;
            }

            if (permissionRecord.HasSameData(permissionRecordInDatabase))
            {
                /* Not changed */
                continue;
            }

            /* Changed */
            permissionRecordInDatabase.Patch(permissionRecord);
            changedRecords.Add(permissionRecordInDatabase);
        }

        /* Deleted */
        var deletedRecords = new List<PermissionDefinitionRecord>();

        if (PermissionOptions.DeletedPermissions.Any())
        {
            deletedRecords.AddRange(
                permissionRecordsInDatabase.Values
                    .Where(x => PermissionOptions.DeletedPermissions.Contains(x.Name))
            );
        }

        if (PermissionOptions.DeletedPermissionGroups.Any())
        {
            deletedRecords.AddIfNotContains(
                permissionRecordsInDatabase.Values
                    .Where(x => PermissionOptions.DeletedPermissionGroups.Contains(x.GroupName))
            );
        }

        if (newRecords.Any())
        {
            newOrChangedPermissions.AddRange(newRecords.Select(x => x.Name));
            await PermissionRepository.InsertManyAsync(newRecords, false, ct);
        }

        if (changedRecords.Any())
        {
            newOrChangedPermissions.AddRange(changedRecords.Select(x => x.Name));
            await PermissionRepository.UpdateManyAsync(changedRecords, false, ct);
        }

        if (deletedRecords.Any())
        {
            await PermissionRepository.DeleteManyAsync(deletedRecords, false, ct);
        }

        return newRecords.Any() || changedRecords.Any() || deletedRecords.Any();
    }
    private PermissionGroupDefinitionRecord MapGroup(
    FrameworkPermissionModel model)
    {
        var item = new PermissionGroupDefinitionRecord();
        item.Name = model.Group;
        item.DisplayName = model.Group;

        return item;
    }
    private IEnumerable<PermissionDefinitionRecord> MapPermissions(
    FrameworkPermissionModel model)
    {
        foreach (var permission in model.Permissions.Distinct())
        {
            var item = new PermissionDefinitionRecord();
            item.Name = permission;
            item.DisplayName = permission;
            item.GroupName = model.Group;
            yield return item;
        }
    }
}

