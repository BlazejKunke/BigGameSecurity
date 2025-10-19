using GameProject.Entities;

namespace GameProject.Tests;

public class GuestTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var guest = new Guest("Riley", TicketType.Vip, 2, TimeSpan.FromMinutes(30));

        Assert.Equal("Riley", guest.Name);
        Assert.Equal(TicketType.Vip, guest.TicketType);
        Assert.Equal(2, guest.PartySize);
        Assert.Equal(TimeSpan.FromMinutes(30), guest.PatienceRemaining);
        Assert.Equal(TimeSpan.FromMinutes(30), guest.InitialPatience);
        Assert.NotEqual(Guid.Empty, guest.Id);
    }

    [Fact]
    public void Wait_ReducesPatienceAndClampsToZero()
    {
        var guest = new Guest("Riley", TicketType.Vip, 2, TimeSpan.FromMinutes(5));

        guest.Wait(TimeSpan.FromMinutes(3));
        Assert.Equal(TimeSpan.FromMinutes(2), guest.PatienceRemaining);
        Assert.False(guest.HasLeftQueue);

        guest.Wait(TimeSpan.FromMinutes(5));
        Assert.Equal(TimeSpan.Zero, guest.PatienceRemaining);
        Assert.True(guest.HasLeftQueue);
    }

    [Fact]
    public void RefreshPatience_RestoresInitialAmount()
    {
        var guest = new Guest("Riley", TicketType.Vip, 2, TimeSpan.FromMinutes(5));

        guest.Wait(TimeSpan.FromMinutes(5));
        guest.RefreshPatience();

        Assert.Equal(guest.InitialPatience, guest.PatienceRemaining);
    }

    [Fact]
    public void GateAssignments_AreTracked()
    {
        var guest = new Guest("Riley", TicketType.Vip, 2, TimeSpan.FromMinutes(5));
        var gate = new Gate("East", 5);

        gate.EnqueueGuest(guest);
        Assert.Equal(gate, guest.CurrentGate);

        var dequeued = gate.DequeueGuest();
        Assert.Equal(guest, dequeued);
        Assert.Null(guest.CurrentGate);
    }
}
