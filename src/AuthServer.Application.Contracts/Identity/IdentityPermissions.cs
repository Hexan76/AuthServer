using Framework.BuildingBlock.Application.Contracts;

namespace AuthServer.Identity;

[FrameworkPermission(nameof(Identity))]
public class IdentityPermissions
{
    private const string GroupName = nameof(Identity);
    public const string Users = $"{GroupName}.{nameof(Users)}";
    public const string Roles = $"{GroupName}.{nameof(Roles)}";

    public const string RoleCreate = $"{Users}.{nameof(RoleCreate)}";
    public const string RoleUpdate = $"{Users}.{nameof(RoleUpdate)}";
    public const string RoleDelete = $"{Users}.{nameof(RoleDelete)}";
    public const string RoleView = $"{Users}.{nameof(RoleView)}";

    public const string UserCreate = $"{Users}.{nameof(UserCreate)}";
    public const string UserUpdate = $"{Users}.{nameof(UserUpdate)}";
    public const string UserDelete = $"{Users}.{nameof(UserDelete)}";
    public const string UserView = $"{Users}.{nameof(UserView)}";
}
