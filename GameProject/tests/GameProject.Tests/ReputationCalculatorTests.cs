using GameProject.Entities;
using GameProject.Game;

namespace GameProject.Tests;

public class ReputationCalculatorTests
{
    [Fact]
    public void Update_WithHighQueues_TriggersPenaltyOncePerInterval()
    {
        var reputationManager = new ReputationManager(20);
        var gateManager = new GateManager(2);
        var calculator = new ReputationCalculator(reputationManager, gateManager);

        foreach (var gate in gateManager.Gates)
        {
            PopulateQueue(gate, 60);
        }

        calculator.Update(TimeSpan.FromHours(18));

        Assert.Equal(15, reputationManager.Score);
        Assert.Single(reputationManager.Events);

        calculator.Update(TimeSpan.FromHours(18).Add(TimeSpan.FromMinutes(1)));

        Assert.Equal(15, reputationManager.Score);
        Assert.Single(reputationManager.Events);
    }

    [Fact]
    public void Update_WithShortQueues_AwardsBonusEveryFifteenMinutes()
    {
        var reputationManager = new ReputationManager();
        var gateManager = new GateManager(1);
        var calculator = new ReputationCalculator(reputationManager, gateManager);

        calculator.Update(TimeSpan.FromHours(18));
        Assert.Equal(2, reputationManager.Score);

        calculator.Update(TimeSpan.FromHours(18).Add(TimeSpan.FromMinutes(10)));
        Assert.Equal(2, reputationManager.Score);

        calculator.Update(TimeSpan.FromHours(18).Add(TimeSpan.FromMinutes(20)));
        Assert.Equal(4, reputationManager.Score);
    }

    [Fact]
    public void ApplyThreatDetected_RewardsMajorThreatStops()
    {
        var reputationManager = new ReputationManager();
        var gateManager = new GateManager(1);
        var calculator = new ReputationCalculator(reputationManager, gateManager);

        var guest = new SecurityGuest(
            "Alex",
            "Rivera",
            30,
            Gender.NonBinary,
            strength: 5,
            aggression: 5,
            hasFakeTicket: false,
            hasFakeId: false,
            isMte: true);

        calculator.ApplyThreatDetected(guest);

        Assert.Equal(20, reputationManager.Score);
        Assert.Contains(reputationManager.Events, e => e.Description.Contains("Major threat"));
    }

    [Fact]
    public void ApplyThreatMissed_PenalizesFakeCredentials()
    {
        var reputationManager = new ReputationManager(10);
        var gateManager = new GateManager(1);
        var calculator = new ReputationCalculator(reputationManager, gateManager);

        var guest = new SecurityGuest(
            "Jordan",
            "Kim",
            28,
            Gender.Female,
            strength: 4,
            aggression: 4,
            hasFakeTicket: true,
            hasFakeId: true,
            isMte: false);

        calculator.ApplyThreatMissed(guest);

        Assert.Equal(0, reputationManager.Score);
        Assert.Contains(reputationManager.Events, e => e.Description.Contains("fake ID"));
    }

    private static void PopulateQueue(SecurityGate gate, int count)
    {
        for (var i = 0; i < count; i++)
        {
            gate.GuestQueue.Enqueue(new SecurityGuest(
                $"First{i}",
                $"Last{i}",
                25,
                Gender.Male,
                strength: 5,
                aggression: 5,
                hasFakeTicket: false,
                hasFakeId: false,
                isMte: false));
        }
    }
}
