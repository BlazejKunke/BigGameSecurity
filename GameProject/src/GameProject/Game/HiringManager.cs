using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GameProject.Entities;

namespace GameProject.Game;

/// <summary>
/// Manages the lifecycle of security staff candidates and the currently hired roster.
/// </summary>
public sealed class HiringManager
{
    private readonly List<SecurityStaff> _availableCVs = new();
    private readonly List<SecurityStaff> _hiredStaff = new();
    private int _nextStaffId = 1;

    /// <summary>
    /// Generates a fresh pool of candidate CVs for the player to review.
    /// </summary>
    /// <param name="count">The number of CVs to create.</param>
    public void GenerateCVPool(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Cannot generate a negative number of CVs.");
        }

        _availableCVs.Clear();

        for (var i = 0; i < count; i++)
        {
            _availableCVs.Add(StatsGenerator.GenerateRandomStaff(_nextStaffId++));
        }
    }

    /// <summary>
    /// Retrieves the next available CV from the pool.
    /// </summary>
    /// <returns>The next candidate, or <c>null</c> if the pool is empty.</returns>
    public SecurityStaff? GetNextCV()
    {
        if (_availableCVs.Count == 0)
        {
            return null;
        }

        var cv = _availableCVs[0];
        _availableCVs.RemoveAt(0);
        return cv;
    }

    /// <summary>
    /// Adds the specified staff member to the hired roster.
    /// </summary>
    public void HireStaff(SecurityStaff staff)
    {
        if (staff is null)
        {
            throw new ArgumentNullException(nameof(staff));
        }

        if (_hiredStaff.Contains(staff))
        {
            return;
        }

        _hiredStaff.Add(staff);
        Console.WriteLine($"\u2713 Hired: {staff.FirstName} {staff.LastName}");
    }

    /// <summary>
    /// Removes the staff member from the hired roster if present.
    /// </summary>
    public void FireStaff(SecurityStaff staff)
    {
        if (staff is null)
        {
            throw new ArgumentNullException(nameof(staff));
        }

        _hiredStaff.Remove(staff);
    }

    /// <summary>
    /// Read-only view of the currently hired staff members.
    /// </summary>
    public IReadOnlyList<SecurityStaff> HiredStaff => new ReadOnlyCollection<SecurityStaff>(_hiredStaff);

    /// <summary>
    /// Number of CVs remaining in the current pool.
    /// </summary>
    public int RemainingCVs => _availableCVs.Count;

    /// <summary>
    /// Indicates whether the roster meets the specified staffing minimum.
    /// </summary>
    public bool HasEnoughStaff(int minimum)
    {
        if (minimum < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(minimum), "Minimum must be non-negative.");
        }

        return _hiredStaff.Count >= minimum;
    }
}
