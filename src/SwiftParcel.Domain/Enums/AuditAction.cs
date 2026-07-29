namespace SwiftParcel.Domain.Enums;

public enum AuditAction
{
    Create,
    StatusChange,
    Assign,
    Update,
    PermissionGrant,
    LoginFailed,
    LoginSucceeded,
    RoleChange,
    Delete,
    Escalate,
    NoteAdd
}