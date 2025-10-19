namespace GameProject.Entities;

/// <summary>
/// Example controllable character with a position and simple movement helper.
/// </summary>
public sealed class Player : Entity
{
    public Player(string name) : base(name)
    {
    }

    public Position2D Position { get; private set; } = new(0, 0);

    public void Move(Position2D delta)
    {
        Position = Position.MoveBy(delta);
    }
}
