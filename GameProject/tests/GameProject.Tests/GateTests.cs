using GameProject.Entities;

namespace GameProject.Tests;

public class GateTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var gate = new Gate("Main", 10);

        Assert.Equal("Main", gate.Name);
        Assert.Equal(10, gate.Capacity);
        Assert.Empty(gate.AssignedStaff);
        Assert.Empty(gate.QueuedGuests);
    }

    [Fact]
    public void AssignStaff_AddsStaffToSet()
    {
        var gate = new Gate("Main", 10);
        var staff = new Staff("Jordan", StaffRole.Security, 90, TimeSpan.FromHours(8));

        gate.AssignStaff(staff);

        Assert.Contains(staff, gate.AssignedStaff);
        Assert.Equal(gate, staff.AssignedGate);
    }

    [Fact]
    public void EnqueueAndDequeueGuest_MaintainsOrder()
    {
        var gate = new Gate("Main", 10);
        var guest1 = new Guest("Riley", TicketType.Vip, 2, TimeSpan.FromMinutes(10));
        var guest2 = new Guest("Avery", TicketType.GeneralAdmission, 1, TimeSpan.FromMinutes(10));

        gate.EnqueueGuest(guest1);
        gate.EnqueueGuest(guest2);

        Assert.Equal(guest1, gate.PeekNextGuest());
        Assert.Equal(guest1, gate.DequeueGuest());
        Assert.Equal(guest2, gate.DequeueGuest());
        Assert.Null(gate.DequeueGuest());
    }

    [Fact]
    public void RemoveStaff_RemovesAssignment()
    {
        var gate = new Gate("Main", 10);
        var staff = new Staff("Jordan", StaffRole.Security, 90, TimeSpan.FromHours(8));

        gate.AssignStaff(staff);
        gate.RemoveStaff(staff);

        Assert.DoesNotContain(staff, gate.AssignedStaff);
        Assert.Null(staff.AssignedGate);
    }
}
