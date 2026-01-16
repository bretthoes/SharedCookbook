namespace SharedCookbook.Application.Common.Mediator;

/// <summary>
/// Represents a void type, since void is not a valid return type in C#.
/// </summary>
public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>, IComparable
{
    /// <summary>
    /// Default and only value of the Unit type
    /// </summary>
    public static readonly Unit Value = new();

    /// <summary>
    /// Task from a Unit value
    /// </summary>
    public static readonly Task<Unit> Task = System.Threading.Tasks.Task.FromResult(Value);

    public int CompareTo(Unit other) => 0;

    int IComparable.CompareTo(object? obj) => 0;

    public override int GetHashCode() => 0;

    public bool Equals(Unit other) => true;

    public override bool Equals(object? obj) => obj is Unit;

    public static bool operator ==(Unit left, Unit right) => true;

    public static bool operator !=(Unit left, Unit right) => false;

    public override string ToString() => "()";
}
