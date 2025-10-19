using GameProject.Entities;
using GameProject.Game;

var gameState = new GameState();

// Create staff members
var staffMembers = new List<Staff>
{
    new("Jordan Lee", StaffRole.Security, 85, TimeSpan.FromHours(8)),
    new("Morgan Patel", StaffRole.Ticketing, 78, TimeSpan.FromHours(6)),
    new("Skyler Chen", StaffRole.Hospitality, 90, TimeSpan.FromHours(7))
};

foreach (var staff in staffMembers)
{
    staff.StartShift();
    gameState.StaffMembers.Add(staff);
}

// Create guests
var guests = new List<Guest>
{
    new("Riley James", TicketType.GeneralAdmission, 2, TimeSpan.FromMinutes(30)),
    new("Casey Morgan", TicketType.Vip, 1, TimeSpan.FromMinutes(45)),
    new("Alex Nguyen", TicketType.SeasonPass, 4, TimeSpan.FromMinutes(35))
};

gameState.Guests.AddRange(guests);

// Create gates and assign staff/guests
var gateA = new Gate("North Gate", capacity: 5);
var gateB = new Gate("South Gate", capacity: 4);

gateA.AssignStaff(staffMembers[0]);
gateA.AssignStaff(staffMembers[1]);

gateB.AssignStaff(staffMembers[2]);

gateA.EnqueueGuest(guests[0]);
gateA.EnqueueGuest(guests[2]);

gateB.EnqueueGuest(guests[1]);

var gates = new[] { gateA, gateB };

gameState.Gates.AddRange(gates);

gameState.TimeManager.Advance(TimeSpan.FromMinutes(15));
gameState.ReputationManager.ApplyPositiveEvent("Gates opened smoothly", 10);

// Print summary
Console.WriteLine("=== Stadium Gate Overview ===\n");
Console.WriteLine($"Current time: {gameState.TimeManager.CurrentTime}");
Console.WriteLine($"Reputation score: {gameState.ReputationManager.Score}\n");

foreach (var gate in gates)
{
    Console.WriteLine($"Gate: {gate.Name} (Capacity: {gate.Capacity})");

    var staffNames = gate.AssignedStaff.Any()
        ? string.Join(", ", gate.AssignedStaff.Select(s => s.Name))
        : "No staff assigned";
    Console.WriteLine($"  Staff: {staffNames}");

    if (gate.PeekNextGuest() is { } nextGuest)
    {
        Console.WriteLine($"  Next guest in queue: {nextGuest.Name} ({nextGuest.TicketType})");
    }
    else
    {
        Console.WriteLine("  No guests in queue");
    }

    Console.WriteLine($"  Total guests waiting: {gate.QueuedGuests.Count}\n");
}

Console.WriteLine("=== Staff Roster ===\n");
foreach (var staff in staffMembers)
{
    var assignment = staff.AssignedGate?.Name ?? "Unassigned";
    Console.WriteLine($"{staff.Name} - {staff.Role} | Skill: {staff.SkillLevel} | Gate: {assignment}");
}

Console.WriteLine("\n=== Guest List ===\n");
foreach (var guest in guests)
{
    var gateName = guest.CurrentGate?.Name ?? "No gate";
    Console.WriteLine($"{guest.Name} - {guest.TicketType} | Party Size: {guest.PartySize} | Gate: {gateName} | Patience remaining: {guest.PatienceRemaining.TotalMinutes} minutes");
}
