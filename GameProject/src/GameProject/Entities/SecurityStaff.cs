using System;

namespace GameProject.Entities;

/// <summary>
/// Specialized security-focused staff member with a nuanced skill profile and reliability metrics.
/// </summary>
public sealed class SecurityStaff : Entity
{
    public SecurityStaff(
        string firstName,
        string lastName,
        int age,
        Gender gender,
        string idNumber,
        int physicalStrength,
        int communication,
        int observation,
        int reliability,
        int focusSustainability,
        int quitRisk) : base(CreateDisplayName(firstName, lastName))
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name is required.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name is required.", nameof(lastName));
        }

        if (age is < 18 or > 80)
        {
            throw new ArgumentOutOfRangeException(nameof(age), "Age must be between 18 and 80.");
        }

        if (string.IsNullOrWhiteSpace(idNumber))
        {
            throw new ArgumentException("An identification number is required.", nameof(idNumber));
        }

        PhysicalStrength = ValidateSkill(physicalStrength, nameof(physicalStrength));
        Communication = ValidateSkill(communication, nameof(communication));
        Observation = ValidateSkill(observation, nameof(observation));

        Reliability = ValidatePercentage(reliability, nameof(reliability));
        FocusSustainability = ValidatePercentage(focusSustainability, nameof(focusSustainability));
        QuitRisk = ValidatePercentage(quitRisk, nameof(quitRisk));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Age = age;
        Gender = gender;
        IdNumber = idNumber.Trim();

        _currentFocus = 100;
        CurrentFocus = 100;
    }

    // Identity
    public string FirstName { get; }
    public string LastName { get; }
    public int Age { get; }
    public Gender Gender { get; }
    public string IdNumber { get; }

    // THREE distinct security skills (1-10 scale)
    public int PhysicalStrength { get; }
    public int Communication { get; }
    public int Observation { get; }

    // Reliability metrics (percentages)
    public int Reliability { get; }
    public int FocusSustainability { get; }
    public int QuitRisk { get; }

    // Dynamic state
    private double _currentFocus;

    public int CurrentFocus { get; private set; }
    public Gate? AssignedGate { get; set; }
    public bool IsOnDuty { get; set; }

    /// <summary>
    /// Adjusts the current focus based on the amount of time elapsed in hours. Focus degrades from 100 down to
    /// the staff member's sustainability rating over the course of six hours.
    /// </summary>
    /// <param name="hoursElapsed">Number of hours since the last update.</param>
    public void UpdateFocus(float hoursElapsed)
    {
        if (hoursElapsed < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(hoursElapsed), "Elapsed time cannot be negative.");
        }

        if (hoursElapsed == 0 || _currentFocus <= FocusSustainability)
        {
            return;
        }

        var degradationPerHour = (100 - FocusSustainability) / 6f;
        var degradation = degradationPerHour * hoursElapsed;
        var updatedFocus = _currentFocus - degradation;

        if (updatedFocus <= FocusSustainability)
        {
            _currentFocus = FocusSustainability;
        }
        else
        {
            _currentFocus = Math.Clamp(updatedFocus, FocusSustainability, 100);
        }

        CurrentFocus = (int)Math.Round(_currentFocus, MidpointRounding.AwayFromZero);
    }

    private static string CreateDisplayName(string firstName, string lastName)
    {
        var first = firstName?.Trim() ?? string.Empty;
        var last = lastName?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(first))
        {
            return last;
        }

        if (string.IsNullOrEmpty(last))
        {
            return first;
        }

        return $"{first} {last}";
    }

    private static int ValidateSkill(int value, string parameterName)
    {
        if (value is < 1 or > 10)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Skill values must be between 1 and 10.");
        }

        return value;
    }

    private static int ValidatePercentage(int value, string parameterName)
    {
        if (value is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Percentage values must be between 0 and 100.");
        }

        return value;
    }
}
