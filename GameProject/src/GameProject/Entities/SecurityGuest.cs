namespace GameProject.Entities;

/// <summary>
/// Represents a guest that goes through the security pipeline and can carry
/// additional risk information that affects the gameplay simulation.
/// </summary>
public sealed class SecurityGuest : Entity
{
    public SecurityGuest(
        string firstName,
        string lastName,
        int age,
        Gender gender,
        int strength,
        int aggression,
        bool hasFakeTicket,
        bool hasFakeId,
        bool isMte)
        : base(BuildName(firstName, lastName))
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name must be provided.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name must be provided.", nameof(lastName));
        }

        if (age < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(age), "Age cannot be negative.");
        }

        if (strength is < 1 or > 10)
        {
            throw new ArgumentOutOfRangeException(nameof(strength), "Strength must be in the range 1-10.");
        }

        if (aggression is < 1 or > 10)
        {
            throw new ArgumentOutOfRangeException(nameof(aggression), "Aggression must be in the range 1-10.");
        }

        FirstName = firstName;
        LastName = lastName;
        Age = age;
        Gender = gender;
        Strength = strength;
        Aggression = aggression;
        HasFakeTicket = hasFakeTicket;
        HasFakeId = hasFakeId;
        IsMTE = isMte;
        CurrentState = GuestState.AtHome;
    }

    public string FirstName { get; }

    public string LastName { get; }

    public int Age { get; }

    public Gender Gender { get; }

    public int Strength { get; }

    public int Aggression { get; }

    public bool HasFakeTicket { get; }

    public bool HasFakeId { get; }

    public bool IsMTE { get; }

    public GuestState CurrentState { get; set; }

    public int? AssignedGateId { get; set; }

    public bool IsHighRisk => HasFakeTicket || HasFakeId || IsMTE || Aggression >= 8;

    private static string BuildName(string firstName, string lastName) => $"{firstName} {lastName}".Trim();
}

public enum GuestState
{
    AtHome,
    AtStadiumGates,
    WalkingToSecurity,
    AtSecurity,
    BeingProcessed,
    WatchingGame,
    Denied,
    AtJail,
    AtHospital,
    HidingAfterGame,
    FightingWithStaff,
    FightingWithGuest
}
