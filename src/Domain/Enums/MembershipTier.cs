namespace SharedCookbook.Domain.Enums;

/// <summary>
/// Predefined membership role for a cookbook. Higher values imply more capability.
/// </summary>
public enum MembershipTier
{
    /// <summary>Read-only access.</summary>
    Viewer = 0,

    /// <summary>Add recipes and invites; edit or delete own recipes only.</summary>
    Contributor = 1,

    /// <summary>Manage members below admin, edit cookbook details, and all recipes.</summary>
    Admin = 2,

    /// <summary>Full control. One owner per cookbook.</summary>
    Owner = 3
}
