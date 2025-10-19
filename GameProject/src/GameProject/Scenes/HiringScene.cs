using GameProject.Entities;
using GameProject.Game;

namespace GameProject.Scenes;

/// <summary>
/// Represents the hiring phase where the player browses CVs and decides who joins the security roster.
/// </summary>
public class HiringScene : IScene
{
    private readonly HiringManager _hiringManager;
    private readonly int _requiredStaffCount;
    private SecurityStaff? _currentCV;

    public HiringScene(HiringManager hiringManager, int requiredStaffCount = 12)
    {
        _hiringManager = hiringManager ?? throw new ArgumentNullException(nameof(hiringManager));
        if (requiredStaffCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(requiredStaffCount), "At least one staff member must be required.");
        }

        _requiredStaffCount = requiredStaffCount;
    }

    public string Name => "Hiring Phase";

    public void LoadContent()
    {
        Console.Clear();
        Console.WriteLine("=== HIRING PHASE ===");
        Console.WriteLine($"You must hire at least {_requiredStaffCount} security staff.\n");

        _hiringManager.GenerateCVPool(30);
        _currentCV = _hiringManager.GetNextCV();
    }

    public void Update(TimeSpan deltaTime, GameState gameState)
    {
        while (!_hiringManager.HasEnoughStaff(_requiredStaffCount))
        {
            if (_currentCV is null)
            {
                Console.WriteLine("\n⚠ No more CVs! Generating more...");
                _hiringManager.GenerateCVPool(10);
                _currentCV = _hiringManager.GetNextCV();
                continue;
            }

            DisplayCV(_currentCV);

            Console.WriteLine("\n[A]ccept  [R]eject");
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.A)
            {
                _hiringManager.HireStaff(_currentCV);
                _currentCV = _hiringManager.GetNextCV();
            }
            else if (key.Key == ConsoleKey.R)
            {
                Console.WriteLine($"✗ Rejected: {_currentCV.FirstName} {_currentCV.LastName}");
                _currentCV = _hiringManager.GetNextCV();
            }

            Console.WriteLine($"\nHired: {_hiringManager.HiredStaff.Count}/{_requiredStaffCount}");
        }
    }

    public void Render()
    {
        Console.WriteLine("\n✓ Hiring complete!");
        Console.WriteLine("=== HIRED ROSTER ===");

        if (_hiringManager.HiredStaff.Count == 0)
        {
            Console.WriteLine("No staff hired.");
            return;
        }

        foreach (var staff in _hiringManager.HiredStaff)
        {
            Console.WriteLine(
                $"- {staff.FirstName} {staff.LastName} | Strength: {staff.PhysicalStrength}, Communication: {staff.Communication}, Observation: {staff.Observation}, Reliability: {staff.Reliability}%");
        }
    }

    private static void DisplayCV(SecurityStaff cv)
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════╗");
        Console.WriteLine("║      APPLICANT RESUME          ║");
        Console.WriteLine("╚════════════════════════════════╝");
        Console.WriteLine($"\nName: {cv.FirstName} {cv.LastName}");
        Console.WriteLine($"Age:  {cv.Age}");
        Console.WriteLine($"ID:   {cv.IdNumber}");
        Console.WriteLine("\n--- SKILLS (1-10) ---");
        Console.WriteLine($"Physical Strength: {cv.PhysicalStrength}");
        Console.WriteLine($"Communication:     {cv.Communication}");
        Console.WriteLine($"Observation:       {cv.Observation}");
        Console.WriteLine("\n--- RELIABILITY ---");
        Console.WriteLine($"Show-up Rate:  {cv.Reliability}%");
        Console.WriteLine($"Focus Endure:  {cv.FocusSustainability}%");
        Console.WriteLine($"Quit Risk:     {cv.QuitRisk}%");
    }
}
