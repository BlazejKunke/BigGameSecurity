using System;
using GameProject.Entities;

namespace GameProject.Game;

/// <summary>
/// Provides helper methods for generating randomized security staff statistics and profiles.
/// </summary>
public static class StatsGenerator
{
    private static readonly Random Random = new();

    private static readonly string[] FirstNames =
    {
        "Jordan", "Casey", "Taylor", "Morgan", "Avery", "Riley", "Quinn", "Hayden", "Peyton", "Skyler"
    };

    private static readonly string[] LastNames =
    {
        "Harris", "Nguyen", "Martinez", "Khan", "Okafor", "Patel", "Ivanov", "Sato", "Fernandez", "O'Neal"
    };

    /// <summary>
    /// Generates a randomized <see cref="SecurityStaff"/> profile using the supplied staff identifier.
    /// </summary>
    /// <param name="staffId">Sequential identifier used to create a stable ID number.</param>
    /// <returns>A new security staff member populated with randomized statistics.</returns>
    public static SecurityStaff GenerateRandomStaff(int staffId)
    {
        var firstName = FirstNames[Random.Next(FirstNames.Length)];
        var lastName = LastNames[Random.Next(LastNames.Length)];
        var age = Random.Next(21, 61); // Reasonable working age range
        var gender = (Gender)Random.Next(Enum.GetValues<Gender>().Length);
        var idNumber = $"SEC-{staffId:D4}";

        var physicalStrength = Random.Next(4, 11);
        var communication = Random.Next(4, 11);
        var observation = Random.Next(4, 11);

        var reliability = Random.Next(60, 101);
        var focusSustainability = Random.Next(50, 101);
        var quitRisk = Random.Next(0, 21);

        return new SecurityStaff(
            firstName,
            lastName,
            age,
            gender,
            idNumber,
            physicalStrength,
            communication,
            observation,
            reliability,
            focusSustainability,
            quitRisk);
    }
}
