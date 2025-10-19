using System;
using System.Collections.Generic;
using System.Linq;
using GameProject.Entities;

namespace GameProject.Game;

/// <summary>
/// Handles random incidents that can occur during the event simulation such as fights and
/// medical emergencies. Incidents impact guest states and the park's reputation score.
/// </summary>
public sealed class IncidentManager
{
    private readonly ReputationManager _reputationManager;
    private readonly List<SecurityGuest> _allGuests;
    private readonly Random _random;

    public IncidentManager(ReputationManager reputationManager, IEnumerable<SecurityGuest> guests)
    {
        _reputationManager = reputationManager ?? throw new ArgumentNullException(nameof(reputationManager));

        if (guests is null)
        {
            throw new ArgumentNullException(nameof(guests));
        }

        _allGuests = guests as List<SecurityGuest> ?? guests.ToList();
        _random = Random.Shared;
    }

    /// <summary>
    /// Processes per-frame incident checks.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last update (currently unused).</param>
    public void Update(TimeSpan deltaTime)
    {
        CheckForFights();
        CheckForMedicalEmergencies();
    }

    private void CheckForFights()
    {
        var aggressiveGuests = _allGuests
            .Where(g => g.CurrentState == GuestState.WatchingGame && g.Aggression >= 7)
            .ToList();

        if (aggressiveGuests.Count < 2)
        {
            return;
        }

        if (_random.NextDouble() >= 0.001)
        {
            return;
        }

        var fighter1Index = _random.Next(aggressiveGuests.Count);
        var fighter2Index = _random.Next(aggressiveGuests.Count - 1);

        if (fighter2Index >= fighter1Index)
        {
            fighter2Index++;
        }

        StartFight(aggressiveGuests[fighter1Index], aggressiveGuests[fighter2Index]);
    }

    private void StartFight(SecurityGuest guest1, SecurityGuest guest2)
    {
        guest1.CurrentState = GuestState.FightingWithGuest;
        guest2.CurrentState = GuestState.FightingWithGuest;

        Console.WriteLine($"⚠ FIGHT: {guest1.FirstName} and {guest2.FirstName} are fighting!");

        _reputationManager.ApplyNegativeEvent("Fight broke out in stadium", 5);

        guest1.CurrentState = GuestState.AtJail;
        guest2.CurrentState = GuestState.AtJail;
    }

    private void CheckForMedicalEmergencies()
    {
        if (_random.NextDouble() >= 0.0001)
        {
            return;
        }

        var eligibleGuests = _allGuests
            .Where(g => g.CurrentState == GuestState.WatchingGame)
            .ToList();

        if (eligibleGuests.Count == 0)
        {
            return;
        }

        var victim = eligibleGuests[_random.Next(eligibleGuests.Count)];
        victim.CurrentState = GuestState.AtHospital;

        Console.WriteLine($"🚑 MEDICAL: {victim.FirstName} needs medical attention!");

        _reputationManager.ApplyNegativeEvent("Medical emergency", 3);
    }
}
