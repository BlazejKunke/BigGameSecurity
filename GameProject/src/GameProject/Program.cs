using GameProject.Game;
using GameProject.Scenes;

Console.WriteLine("╔════════════════════════════════════════════╗");
Console.WriteLine("║         BIGGAMESECURITY v1.0               ║");
Console.WriteLine("║   Stadium Security Management Simulator    ║");
Console.WriteLine("╚════════════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine("Welcome, Director!");
Console.WriteLine();
Console.WriteLine("Your role: Oversee gate operations for tonight's big game.");
Console.WriteLine("Your goal: Maintain stadium security and reputation above zero.");
Console.WriteLine();
Console.WriteLine("Press any key to begin hiring staff...");
Console.ReadKey(true);

// Initialize core managers (shared across all events)
var timeManager = new TimeManager(DateTime.Today.AddHours(18)); // Start at 18:00
var reputationManager = new ReputationManager(startingScore: 100);
var hiringManager = new HiringManager();
var gateManager = new GateManager(gateCount: 4); // Start with 4 gates for simplicity

// Game loop - can play multiple events
var eventNumber = 1;
var continuePlay = true;

while (continuePlay && reputationManager.Score > 0)
{
    Console.Clear();
    Console.WriteLine($"════════════════════════════════════════════");
    Console.WriteLine($"              EVENT #{eventNumber}");
    Console.WriteLine($"════════════════════════════════════════════");
    Console.WriteLine();

    // Phase 1: Hiring (if needed)
    if (!hiringManager.HasEnoughStaff(gateManager.Gates.Count))
    {
        var hiringScene = new HiringScene(hiringManager, requiredStaffCount: gateManager.Gates.Count);
        hiringScene.LoadContent();
        hiringScene.Update(TimeSpan.Zero, null!); // Blocking scene
        hiringScene.Render();
        Console.WriteLine("\nPress any key to continue to staff assignment...");
        Console.ReadKey();
    }

    // Phase 2: Assignment
    var assignmentScene = new AssignmentScene(hiringManager, gateManager);
    assignmentScene.LoadContent();
    assignmentScene.Update(TimeSpan.Zero, null!); // Blocking scene
    assignmentScene.Render();
    Console.WriteLine("\nPress any key to start the event...");
    Console.ReadKey();

    // Phase 3: Event Execution (real-time simulation)
    // Reset time for new event
    timeManager = new TimeManager(DateTime.Today.AddHours(18));
    
    var guestFlowManager = new GuestFlowManager(gateManager);
    var incidentManager = new IncidentManager(reputationManager, guestFlowManager.AllGuests);
    var reputationCalculator = new ReputationCalculator(reputationManager, gateManager);
    
    var eventScene = new EventScene(
        timeManager,
        gateManager,
        guestFlowManager,
        incidentManager,
        reputationManager,
        defaultGuestCount: 300); // Simplified guest count

    eventScene.LoadContent();

    // Run event in real-time simulation
    var lastUpdateTime = DateTime.Now;
    var eventComplete = false;

    while (!eventComplete && reputationManager.Score > 0)
    {
        var currentTime = DateTime.Now;
        var deltaTime = currentTime - lastUpdateTime;
        lastUpdateTime = currentTime;

        // Update simulation
        eventScene.Update(deltaTime, null!);
        
        // Apply reputation effects from gate performance
        reputationCalculator.Update(timeManager.CurrentTime.TimeOfDay);

        // Render current state
        eventScene.Render();

        // Check if event is complete (reached 24:00)
        if (timeManager.CurrentTime.TimeOfDay >= TimeSpan.FromHours(24))
        {
            eventComplete = true;
        }

        // Small delay to prevent CPU spinning (target ~30 FPS)
        Thread.Sleep(33);
    }

    // Check for game over
    if (reputationManager.Score <= 0)
    {
        Console.Clear();
        Console.WriteLine();
        Console.WriteLine("════════════════════════════════════════════");
        Console.WriteLine("              GAME OVER");
        Console.WriteLine("════════════════════════════════════════════");
        Console.WriteLine();
        Console.WriteLine("Your reputation has dropped to zero.");
        Console.WriteLine("You have been terminated from your position.");
        Console.WriteLine();
        Console.WriteLine($"Events Completed: {eventNumber - 1}");
        Console.WriteLine($"Total Guests Processed: {guestFlowManager.TotalGuestsProcessed}");
        Console.WriteLine();
        Console.WriteLine("Thank you for playing BigGameSecurity!");
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
        break;
    }

    // Phase 4: Post-Event Review
    var postEventScene = new PostEventScene(
        reputationManager,
        gateManager,
        guestFlowManager,
        hiringManager);

    postEventScene.LoadContent();
    postEventScene.Render();

    // User decides whether to continue
    Console.WriteLine("\nDo you want to continue to the next event? [Y/N]");
    var response = Console.ReadKey(true);
    continuePlay = response.Key == ConsoleKey.Y;

    eventNumber++;
}

Console.Clear();
Console.WriteLine();
Console.WriteLine("════════════════════════════════════════════");
Console.WriteLine("         Thank you for playing!");
Console.WriteLine("════════════════════════════════════════════");
Console.WriteLine();
Console.WriteLine($"Final Statistics:");
Console.WriteLine($"  Events Completed: {eventNumber - 1}");
Console.WriteLine($"  Final Reputation: {reputationManager.Score}");
Console.WriteLine();
Console.WriteLine("BigGameSecurity v1.0 - Stadium Security Management Simulator");
Console.WriteLine();
