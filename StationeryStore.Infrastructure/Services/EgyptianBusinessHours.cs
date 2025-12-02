using System;
using System.Collections.Generic;
using StationeryStore.Application.Interfaces;

namespace StationeryStore.Infrastructure.Services;

public class EgyptianBusinessHours : IEgyptianBusinessHours
{
    public IReadOnlyCollection<DayOfWeek> ClosedDays { get; }
    public TimeSpan OpeningTime { get; }
    public TimeSpan ClosingTime { get; }

    public EgyptianBusinessHours()
    {
        ClosedDays = new[] { DayOfWeek.Sunday };
        OpeningTime = new TimeSpan(9, 0, 0);  // 09:00
        ClosingTime = new TimeSpan(22, 0, 0); // 22:00
    }

    public bool IsWorkingDay(DateTime date) => !ClosedDays.Contains(date.DayOfWeek);

    public bool IsOpenAt(DateTime dateTime)
    {
        if (!IsWorkingDay(dateTime)) return false;
        var time = dateTime.TimeOfDay;
        if (OpeningTime <= ClosingTime) return time >= OpeningTime && time < ClosingTime;
        return time >= OpeningTime || time < ClosingTime;
    }
}