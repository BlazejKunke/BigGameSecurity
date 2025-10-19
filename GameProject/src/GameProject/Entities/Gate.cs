using System.Collections.ObjectModel;
using System.Linq;

namespace GameProject.Entities;

/// <summary>
/// Represents a stadium gate where guests queue for entry and staff are assigned to manage the flow.
/// </summary>
public sealed class Gate : Entity
{
    private readonly Queue<Guest> _queue = new();
    private readonly HashSet<Staff> _assignedStaff = new();

    public Gate(string name, int capacity) : base(name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A gate must have a name.", nameof(name));
        }

        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be at least 1.");
        }

        Capacity = capacity;
    }

    /// <summary>
    /// Maximum number of guests that can be processed during a single batch.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Read-only view of the guests currently in queue.
    /// </summary>
    public IReadOnlyCollection<Guest> QueuedGuests => new ReadOnlyCollection<Guest>(_queue.ToArray());

    /// <summary>
    /// Staff currently working this gate.
    /// </summary>
    public IReadOnlyCollection<Staff> AssignedStaff => _assignedStaff.ToList().AsReadOnly();

    public void AssignStaff(Staff staff)
    {
        if (staff is null)
        {
            throw new ArgumentNullException(nameof(staff));
        }

        if (staff.AssignedGate is not null && staff.AssignedGate != this)
        {
            throw new InvalidOperationException($"{staff.Name} is already assigned to gate {staff.AssignedGate.Name}.");
        }

        if (_assignedStaff.Add(staff))
        {
            staff.AssignGate(this);
        }
    }

    public void RemoveStaff(Staff staff)
    {
        if (staff is null)
        {
            throw new ArgumentNullException(nameof(staff));
        }

        if (_assignedStaff.Remove(staff))
        {
            staff.UnassignGate();
        }
    }

    public void EnqueueGuest(Guest guest)
    {
        if (guest is null)
        {
            throw new ArgumentNullException(nameof(guest));
        }

        if (guest.CurrentGate is not null && guest.CurrentGate != this)
        {
            throw new InvalidOperationException($"{guest.Name} is already queued at gate {guest.CurrentGate.Name}.");
        }

        _queue.Enqueue(guest);
        guest.AssignGate(this);
    }

    public Guest? DequeueGuest()
    {
        if (_queue.Count == 0)
        {
            return null;
        }

        var guest = _queue.Dequeue();
        guest.LeaveGate();
        return guest;
    }

    public Guest? PeekNextGuest() => _queue.Count == 0 ? null : _queue.Peek();
}
