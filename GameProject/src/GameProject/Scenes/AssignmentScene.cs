using GameProject.Game;

namespace GameProject.Scenes;

/// <summary>
/// Scene responsible for letting the player assign hired staff members to specific security gates.
/// </summary>
public class AssignmentScene : IScene
{
    private readonly HiringManager _hiringManager;
    private readonly GateManager _gateManager;

    public AssignmentScene(HiringManager hiringManager, GateManager gateManager)
    {
        _hiringManager = hiringManager ?? throw new ArgumentNullException(nameof(hiringManager));
        _gateManager = gateManager ?? throw new ArgumentNullException(nameof(gateManager));
    }

    public string Name => "Staff Assignment";

    public void LoadContent()
    {
        Console.Clear();
        Console.WriteLine("=== STAFF ASSIGNMENT ===");
    }

    public void Update(TimeSpan deltaTime, GameState gameState)
    {
        if (_hiringManager.HiredStaff.Count == 0)
        {
            Console.WriteLine("No staff are currently hired. Hire staff before assigning them to gates.");
            return;
        }

        if (_hiringManager.HiredStaff.Count < _gateManager.Gates.Count)
        {
            Console.WriteLine("Not enough staff to cover every gate. Hire additional staff before continuing.");
            return;
        }

        while (!_gateManager.AreAllGatesStaffed())
        {
            DisplayGatesAndStaff();

            Console.WriteLine("\nEnter staff number and gate number (e.g., '1 3' assigns staff 1 to gate 3):");
            var input = Console.ReadLine()?.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (input?.Length != 2 ||
                !int.TryParse(input[0], out var staffIndex) ||
                !int.TryParse(input[1], out var gateId))
            {
                Console.WriteLine("Invalid input. Please enter two numbers separated by a space.");
                continue;
            }

            if (staffIndex is < 1 or > _hiringManager.HiredStaff.Count)
            {
                Console.WriteLine("That staff number is out of range. Try again.");
                continue;
            }

            var gate = _gateManager.Gates.FirstOrDefault(g => g.Id == gateId);
            if (gate is null)
            {
                Console.WriteLine($"Gate {gateId} does not exist. Try again.");
                continue;
            }

            var staff = _hiringManager.HiredStaff[staffIndex - 1];
            if (staff.AssignedGate?.Id == gate.Id)
            {
                Console.WriteLine($"{staff.FirstName} is already assigned to Gate {gate.Id}.");
                continue;
            }

            _gateManager.AssignStaffToGate(staff, gate.Id);
            Console.WriteLine($"\u2713 Assigned {staff.FirstName} to Gate {gate.Id}");
        }
    }

    public void Render()
    {
        Console.WriteLine("\n\u2713 All gates staffed! Ready to start event.");
    }

    private void DisplayGatesAndStaff()
    {
        Console.WriteLine("\nCurrent Gate Coverage:");
        foreach (var gate in _gateManager.Gates)
        {
            var staffNames = gate.AssignedStaff.Count == 0
                ? "Unstaffed"
                : string.Join(", ", gate.AssignedStaff.Select(s => $"{s.FirstName} {s.LastName}"));

            Console.WriteLine($"  Gate {gate.Id}: {staffNames}");
        }

        Console.WriteLine("\nStaff Roster:");
        for (var i = 0; i < _hiringManager.HiredStaff.Count; i++)
        {
            var staff = _hiringManager.HiredStaff[i];
            var assignment = staff.AssignedGate is null ? "Unassigned" : $"Gate {staff.AssignedGate.Id}";
            Console.WriteLine($"  {i + 1}. {staff.FirstName} {staff.LastName} - {assignment}");
        }
    }
}
