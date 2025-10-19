using GameProject.Entities;

namespace GameProject.Tests;

public class StaffTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var staff = new Staff("Jordan", StaffRole.Security, 90, TimeSpan.FromHours(8));

        Assert.Equal("Jordan", staff.Name);
        Assert.Equal(StaffRole.Security, staff.Role);
        Assert.Equal(90, staff.SkillLevel);
        Assert.Equal(TimeSpan.FromHours(8), staff.ShiftLength);
        Assert.NotEqual(Guid.Empty, staff.Id);
        Assert.False(staff.IsOnDuty);
    }

    [Fact]
    public void StartAndEndShift_UpdateDutyStatus()
    {
        var staff = new Staff("Jordan", StaffRole.Security, 90, TimeSpan.FromHours(8));

        staff.StartShift();
        Assert.True(staff.IsOnDuty);

        staff.EndShift();
        Assert.False(staff.IsOnDuty);
    }

    [Fact]
    public void AdjustSkill_ClampsValues()
    {
        var staff = new Staff("Jordan", StaffRole.Security, 90, TimeSpan.FromHours(8));

        staff.AdjustSkill(15);
        Assert.Equal(100, staff.SkillLevel);

        staff.AdjustSkill(-250);
        Assert.Equal(0, staff.SkillLevel);
    }

    [Fact]
    public void AssignGate_SetsGateReference()
    {
        var staff = new Staff("Jordan", StaffRole.Security, 90, TimeSpan.FromHours(8));
        var gate = new Gate("East", 5);

        gate.AssignStaff(staff);

        Assert.Equal(gate, staff.AssignedGate);

        gate.RemoveStaff(staff);
        Assert.Null(staff.AssignedGate);
    }
}
