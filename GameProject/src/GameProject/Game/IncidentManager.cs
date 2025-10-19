using System.Collections.ObjectModel;
using System.Linq;
using GameProject.Entities;

namespace GameProject.Game;

/// <summary>
/// Handles random incidents that can occur at gates during the event and applies
/// the appropriate consequences to the reputation system.
/// </summary>
public sealed class IncidentManager
{
    private readonly GateManager _gateManager;
    private readonly ReputationManager _reputationManager;
    private readonly List<Incident> _recentIncidents = new();
    private readonly TimeSpan _evaluationInterval;
    private TimeSpan _timeAccumulator;

    public IncidentManager(GateManager gateManager, ReputationManager reputationManager, TimeSpan? evaluationInterval = null)
    {
        _gateManager = gateManager ?? throw new ArgumentNullException(nameof(gateManager));
        _reputationManager = reputationManager ?? throw new ArgumentNullException(nameof(reputationManager));
        _evaluationInterval = evaluationInterval ?? TimeSpan.FromMinutes(5);
    }

    /// <summary>
    /// Raised whenever a new incident is generated.
    /// </summary>
    public event EventHandler<Incident>? IncidentOccurred;

    /// <summary>
    /// Most recent incidents, ordered by occurrence.
    /// </summary>
    public IReadOnlyList<Incident> RecentIncidents => new ReadOnlyCollection<Incident>(_recentIncidents);

    /// <summary>
    /// Advances the incident simulation.
    /// </summary>
    public void Update(TimeSpan deltaTime)
    {
        if (deltaTime < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(deltaTime), "Delta time cannot be negative.");
        }

        if (deltaTime == TimeSpan.Zero)
        {
            return;
        }

        _timeAccumulator += deltaTime;

        while (_timeAccumulator >= _evaluationInterval)
        {
            _timeAccumulator -= _evaluationInterval;
            EvaluateGatesForIncidents();
        }
    }

    private void EvaluateGatesForIncidents()
    {
        foreach (var gate in _gateManager.Gates)
        {
            if (!gate.IsOpen)
            {
                continue;
            }

            var queueLength = gate.QueueLength;

            if (queueLength == 0)
            {
                continue;
            }

            var focusAverage = gate.AssignedStaff.Count == 0
                ? 0f
                : (float)gate.AssignedStaff.Average(staff => staff.CurrentFocus);

            // Higher queues and lower focus both increase the likelihood of incidents.
            var queuePressure = Math.Clamp(queueLength / 75f, 0f, 1f);
            var fatigueFactor = 1f - Math.Clamp(focusAverage / 100f, 0f, 1f);
            var incidentProbability = Math.Clamp((queuePressure * 0.6f) + (fatigueFactor * 0.4f), 0.05f, 0.95f);

            if (Random.Shared.NextDouble() > incidentProbability)
            {
                continue;
            }

            var severity = (int)Math.Clamp(Math.Round(incidentProbability * 10), 1, 10);
            var description = severity switch
            {
                >= 8 => $"Major disturbance at gate {gate.Id}",
                >= 5 => $"Escalated confrontation at gate {gate.Id}",
                _ => $"Minor scuffle at gate {gate.Id}"
            };

            _reputationManager.ApplyNegativeEvent(description, severity);

            var incident = new Incident(DateTime.UtcNow, gate.Id, description, severity, queueLength);
            _recentIncidents.Add(incident);

            // Keep the list trimmed so the HUD remains readable.
            const int maxIncidentsTracked = 20;
            if (_recentIncidents.Count > maxIncidentsTracked)
            {
                _recentIncidents.RemoveAt(0);
            }

            IncidentOccurred?.Invoke(this, incident);
        }
    }
}

/// <summary>
/// Describes a single incident that occurred during the event.
/// </summary>
/// <param name="Timestamp">The UTC time when the incident took place.</param>
/// <param name="GateId">Identifier of the affected gate.</param>
/// <param name="Description">Human readable description of the incident.</param>
/// <param name="Severity">How severe the incident was on a 1-10 scale.</param>
/// <param name="QueueLength">Number of guests that were waiting when the incident happened.</param>
public sealed record Incident(DateTime Timestamp, int GateId, string Description, int Severity, int QueueLength);
