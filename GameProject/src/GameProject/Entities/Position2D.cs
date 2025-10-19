namespace GameProject.Entities;

/// <summary>
/// Immutable integer-based position with helpers for basic movement.
/// </summary>
public readonly record struct Position2D(int X, int Y)
{
    public Position2D MoveBy(Position2D delta) => new(X + delta.X, Y + delta.Y);

    public override string ToString() => $"({X}, {Y})";
}
