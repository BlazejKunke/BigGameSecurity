using System;
using System.Collections.Generic;
using System.Linq;
using GameProject.Entities;

namespace GameProject.Game;

/// <summary>
/// Coordinates the stadium security gates, allowing bulk operations and staffing checks.
/// </summary>
public sealed class GateManager
{
    private readonly List<SecurityGate> _gates;

    public GateManager(int gateCount)
    {
        if (gateCount is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(gateCount), "Gate count must be between 1 and 12.");
        }

        _gates = new List<SecurityGate>(gateCount);

        for (var i = 0; i < gateCount; i++)
        {
            _gates.Add(new SecurityGate(i + 1));
        }
    }

    public IReadOnlyList<SecurityGate> Gates => _gates;

    public void OpenAllGates()
    {
        foreach (var gate in _gates)
        {
            gate.IsOpen = true;
        }
    }

    public void CloseAllGates()
    {
        foreach (var gate in _gates)
        {
            gate.IsOpen = false;
        }
    }

    public bool AreAllGatesStaffed()
    {
        return _gates.All(gate => gate.AssignedStaff.Count > 0);
    }

    public void AssignStaffToGate(SecurityStaff staff, int gateId)
    {
        if (staff is null)
        {
            throw new ArgumentNullException(nameof(staff));
        }

        var gate = _gates.FirstOrDefault(g => g.Id == gateId)
                   ?? throw new ArgumentException($"Gate {gateId} not found", nameof(gateId));

        if (staff.AssignedGate is not null)
        {
            staff.AssignedGate.AssignedStaff.Remove(staff);
        }

        if (!gate.AssignedStaff.Contains(staff))
        {
            gate.AssignedStaff.Add(staff);
        }

        staff.AssignedGate = gate;
    }

    public int TotalQueueLength => _gates.Sum(gate => gate.QueueLength);
}
