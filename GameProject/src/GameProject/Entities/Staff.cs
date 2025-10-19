using System.Diagnostics.CodeAnalysis;

namespace GameProject.Entities;

/// <summary>
/// Represents a staff member working inside the stadium. Staff members can be assigned to a
/// gate and have metadata that describes their skills and availability.
/// </summary>
public sealed class Staff : Entity
{
    public Staff(string name, StaffRole role, int skillLevel, TimeSpan shiftLength) : base(name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A staff member must have a name.", nameof(name));
        }

        if (skillLevel is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(skillLevel), "Skill level must be between 0 and 100.");
        }

        if (shiftLength <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(shiftLength), "Shift length must be a positive duration.");
        }

        Role = role;
        SkillLevel = skillLevel;
        ShiftLength = shiftLength;
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Unique identifier that can be used for persistence or lookups.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// The job specialization for the staff member.
    /// </summary>
    public StaffRole Role { get; }

    /// <summary>
    /// A simple 0-100 scale describing the staff member's proficiency.
    /// </summary>
    public int SkillLevel { get; private set; }

    /// <summary>
    /// Duration of the shift the staff member is expected to work.
    /// </summary>
    public TimeSpan ShiftLength { get; private set; }

    /// <summary>
    /// Indicates whether the staff member is actively working.
    /// </summary>
    public bool IsOnDuty { get; private set; }

    /// <summary>
    /// The gate the staff member is currently supporting, if any.
    /// </summary>
    public Gate? AssignedGate { get; private set; }

    public void StartShift() => IsOnDuty = true;

    public void EndShift()
    {
        IsOnDuty = false;
        UnassignGate();
    }

    /// <summary>
    /// Updates the skill level to reflect training or demotions.
    /// </summary>
    public void AdjustSkill(int delta)
    {
        SkillLevel = Math.Clamp(SkillLevel + delta, 0, 100);
    }

    internal void AssignGate(Gate gate)
    {
        AssignedGate = gate;
    }

    internal void UnassignGate()
    {
        AssignedGate = null;
    }
}

/// <summary>
/// High-level roles that staff members can perform.
/// </summary>
[SuppressMessage("Design", "CA1027:Mark enums with FlagsAttribute", Justification = "Roles are mutually exclusive states.")]
public enum StaffRole
{
    Security,
    Ticketing,
    Hospitality,
    Maintenance,
    Management
}
