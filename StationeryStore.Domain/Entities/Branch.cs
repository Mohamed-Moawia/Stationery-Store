// StationeryStore.Domain/Entities/Branch.cs
using System;
using System.Collections.Generic;
using StationeryStore.Domain.Common;

namespace StationeryStore.Domain.Entities;

public class Branch : BaseEntity
{
    // Required for Egyptian businesses
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;

    public string AddressAr { get; set; } = string.Empty;
    public string AddressEn { get; set; } = string.Empty;

    // Egyptian Tax Authority (ETA) Registration
    public string? TaxRegistrationNumber { get; set; } // الرقم الضريبي
    public string? TaxActivityCode { get; set; } // رمز النشاط
    public string? CommercialRegistration { get; set; } // السجل التجاري

    // Location for Egyptian VAT purposes
    public string GovernorateAr { get; set; } = string.Empty; // المحافظة
    public string GovernorateEn { get; set; } = string.Empty;
    public string CityAr { get; set; } = string.Empty; // المدينة
    public string CityEn { get; set; } = string.Empty;

    // Contact info in Egyptian format
    public string Phone { get; set; } = string.Empty; // +20XXXXXXXXXX
    public string? SecondaryPhone { get; set; }
    public string? Email { get; set; }

    // Business hours (Egyptian work week: Sunday is weekend; other days are working days)
    // Default working hours: 09:00 - 22:00
    public TimeSpan OpeningTime { get; set; } = new TimeSpan(9, 0, 0);  // 09:00
    public TimeSpan ClosingTime { get; set; } = new TimeSpan(22, 0, 0); // 22:00

    // Default closed day(s). Egypt weekend specified as Sunday only.
    // Immutable read-only collection to avoid accidental mutation
    public IReadOnlyCollection<DayOfWeek> ClosedDays { get; init; } =
        Array.AsReadOnly(new[] { DayOfWeek.Sunday });

    // Returns true if the branch is a working day (not a closed-day)
    public bool IsWorkingDay(DayOfWeek day) => !ClosedDays.Contains(day);

    // Returns true if the branch is open at a given DateTime (handles overnight hours)
    // Note: Ensure the provided dateTime is in the branch's local timezone.
    public bool IsOpenAt(DateTime dateTime)
    {
        if (!IsWorkingDay(dateTime.DayOfWeek))
            return false;

        var time = dateTime.TimeOfDay;

        // Normal schedule (same day)
        if (OpeningTime <= ClosingTime)
            return time >= OpeningTime && time < ClosingTime;

        // Overnight schedule (e.g., open 22:00 - 06:00)
        return time >= OpeningTime || time < ClosingTime;
    }

    public bool IsOpenAt(DateTimeOffset dateTime)
    {
        var local = dateTime.ToLocalTime();
        if (!IsWorkingDay(local.DayOfWeek)) return false;
        var time = local.TimeOfDay;
        if (OpeningTime <= ClosingTime) return time >= OpeningTime && time < ClosingTime;
        return time >= OpeningTime || time < ClosingTime;
    }

    public bool IsOpenNow(TimeZoneInfo tz)
    {
        return IsOpenAt(DateTimeOffset.UtcNow.ToOffset(tz.GetUtcOffset(DateTime.UtcNow)));
    }

    public bool IsActive { get; set; } = true;
    public bool IsHeadquarters { get; set; } = false;

    // Navigation properties
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<Till> Tills { get; set; } = new List<Till>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}