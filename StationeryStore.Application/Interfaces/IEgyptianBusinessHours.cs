using System;
using System.Collections.Generic;

namespace StationeryStore.Application.Interfaces;

public interface IEgyptianBusinessHours
{
    IReadOnlyCollection<DayOfWeek> ClosedDays { get; }
    TimeSpan OpeningTime { get; }
    TimeSpan ClosingTime { get; }

    bool IsWorkingDay(DateTime date);
    bool IsOpenAt(DateTime dateTime);
}