using System;
using System.Collections.Generic;
using System.Linq;
using GameProject.Entities;

namespace GameProject.Game;

/// <summary>
/// Coordinates the lifecycle of guests entering the stadium. Responsible for generating guests,
/// spawning them at realistic time windows, routing them toward the optimal security gate, and
/// handing off processing work to gates based on the staffing performance metrics.
/// </summary>
public sealed class GuestFlowManager
{
    private readonly GateManager _gateManager;
    private readonly List<SecurityGuest> _allGuests;
    private readonly Dictionary<int, TimeSpan> _gateProcessingTimers;
    private int _nextGuestId;

    public GuestFlowManager(GateManager gateManager)
    {
        _gateManager = gateManager ?? throw new ArgumentNullException(nameof(gateManager));
        _allGuests = new List<SecurityGuest>();
        _gateProcessingTimers = gateManager.Gates.ToDictionary(g => g.Id, _ => TimeSpan.Zero);
        _nextGuestId = 1;
    }

    /// <summary>
    /// Generates a fresh set of guests for the upcoming event.
    /// </summary>
    /// <param name="totalCount">Total number of guests to create.</param>
    public void GenerateGuests(int totalCount)
    {
        if (totalCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalCount), "Guest count cannot be negative.");
        }

        _allGuests.Clear();
        _nextGuestId = 1;

        for (var i = 0; i < totalCount; i++)
        {
            var guest = GuestGenerator.GenerateRandomGuest(_nextGuestId++);
            _allGuests.Add(guest);
        }
    }

    /// <summary>
    /// Primary update loop invoked by the game. Manages spawning, routing and processing for the
    /// current frame.
    /// </summary>
    /// <param name="currentGameTime">Absolute time within the game day.</param>
    /// <param name="deltaTime">Time elapsed since the previous update call.</param>
    public void Update(TimeSpan currentGameTime, TimeSpan deltaTime)
    {
        if (deltaTime < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(deltaTime), "Delta time cannot be negative.");
        }

        SpawnGuestsForCurrentTime(currentGameTime);
        AssignGuestsToGates();
        ProcessGuestsAtGates(deltaTime);
    }

    private void SpawnGuestsForCurrentTime(TimeSpan currentTime)
    {
        // 18:00-18:45: Guests accumulate outside
        if (currentTime >= TimeSpan.FromHours(18) && currentTime < TimeSpan.FromMinutes(1125))
        {
            SpawnGuests(10, 20);
        }

        // 18:45-20:30: Peak rush (most guests spawn)
        if (currentTime >= TimeSpan.FromMinutes(1125) && currentTime < TimeSpan.FromMinutes(1230))
        {
            SpawnGuests(30, 50);
        }

        // 20:30-21:30: Late arrivals trickle in
        if (currentTime >= TimeSpan.FromMinutes(1230) && currentTime < TimeSpan.FromMinutes(1290))
        {
            SpawnGuests(5, 10);
        }
    }

    private void SpawnGuests(int minimum, int maximumInclusive)
    {
        var readyGuests = _allGuests
            .Where(g => g.CurrentState == GuestState.AtHome)
            .Take(Random.Shared.Next(minimum, maximumInclusive + 1))
            .ToList();

        foreach (var guest in readyGuests)
        {
            guest.CurrentState = GuestState.AtStadiumGates;
        }
    }

    private void AssignGuestsToGates()
    {
        var waitingGuests = _allGuests
            .Where(g => g.CurrentState == GuestState.AtStadiumGates)
            .ToList();

        if (waitingGuests.Count == 0)
        {
            return;
        }

        var openGates = _gateManager.Gates.Where(g => g.IsOpen).ToList();

        if (openGates.Count == 0)
        {
            return;
        }

        foreach (var guest in waitingGuests)
        {
            var shortestQueueGate = openGates
                .OrderBy(g => g.QueueLength)
                .ThenBy(g => g.Id)
                .FirstOrDefault();

            if (shortestQueueGate is null)
            {
                break;
            }

            shortestQueueGate.GuestQueue.Enqueue(guest);
            guest.CurrentState = GuestState.AtSecurity;
            guest.AssignedGateId = shortestQueueGate.Id;
        }
    }

    private void ProcessGuestsAtGates(TimeSpan deltaTime)
    {
        foreach (var gate in _gateManager.Gates)
        {
            if (!_gateProcessingTimers.ContainsKey(gate.Id))
            {
                _gateProcessingTimers[gate.Id] = TimeSpan.Zero;
            }

            if (!gate.IsOpen || gate.GuestQueue.Count == 0)
            {
                _gateProcessingTimers[gate.Id] = TimeSpan.Zero;
                continue;
            }

            if (gate.AverageObservation <= 0)
            {
                continue;
            }

            var processingSpeed = Math.Clamp(gate.AverageObservation / 10f, 0.1f, 2.5f);
            var secondsPerGuest = TimeSpan.FromSeconds(5f / processingSpeed);
            var accumulated = _gateProcessingTimers[gate.Id] + deltaTime;

            while (accumulated >= secondsPerGuest && gate.GuestQueue.Count > 0)
            {
                gate.ProcessNextGuest();
                accumulated -= secondsPerGuest;
            }

            _gateProcessingTimers[gate.Id] = accumulated;
        }
    }

    public int TotalGuestsProcessed => _allGuests.Count(g => g.CurrentState == GuestState.WatchingGame);

    public int TotalGuestsWaiting => _allGuests.Count(g =>
        g.CurrentState == GuestState.AtStadiumGates || g.CurrentState == GuestState.AtSecurity);

    public IReadOnlyList<SecurityGuest> AllGuests => _allGuests;
}
