using System;
using GameProject.Entities;

namespace GameProject.Game;

/// <summary>
/// Utility responsible for creating randomized <see cref="SecurityGuest"/> instances that feed the
/// guest flow simulation. Generates plausible demographics alongside random risk traits so that the
/// security pipeline can be stress-tested without handcrafted data.
/// </summary>
public static class GuestGenerator
{
    private static readonly string[] FirstNames =
    {
        "Alex", "Jordan", "Taylor", "Morgan", "Riley", "Casey", "Drew", "Skylar", "Rowan", "Avery"
    };

    private static readonly string[] LastNames =
    {
        "Johnson", "Martinez", "Okoro", "Singh", "Petrov", "Smith", "Chen", "Garcia", "Brown", "Kim"
    };

    /// <summary>
    /// Creates a randomized security guest profile that can be used in the simulation.
    /// </summary>
    /// <param name="guestId">Sequential identifier used to create unique names.</param>
    /// <returns>A new <see cref="SecurityGuest"/> populated with randomized statistics.</returns>
    public static SecurityGuest GenerateRandomGuest(int guestId)
    {
        if (guestId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(guestId), "Guest id must be positive.");
        }

        var firstName = FirstNames[Random.Shared.Next(FirstNames.Length)];
        var lastName = LastNames[Random.Shared.Next(LastNames.Length)];

        // Add the sequential identifier to reduce the likelihood of duplicate full names.
        lastName = $"{lastName}-{guestId:D4}";

        var age = Random.Shared.Next(16, 81);
        var gender = (Gender)Random.Shared.Next(Enum.GetValues<Gender>().Length);
        var strength = Random.Shared.Next(1, 11);
        var aggression = Random.Shared.Next(1, 11);
        var hasFakeTicket = Random.Shared.NextDouble() < 0.08; // 8% chance
        var hasFakeId = Random.Shared.NextDouble() < 0.05; // 5% chance
        var isMte = Random.Shared.NextDouble() < 0.01; // Rare edge-case guests

        return new SecurityGuest(
            firstName,
            lastName,
            age,
            gender,
            strength,
            aggression,
            hasFakeTicket,
            hasFakeId,
            isMte);
    }
}
