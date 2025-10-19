namespace GameProject.Entities;

/// <summary>
/// Base type for every interactive element in the game world.
/// </summary>
public abstract class Entity
{
    protected Entity(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
