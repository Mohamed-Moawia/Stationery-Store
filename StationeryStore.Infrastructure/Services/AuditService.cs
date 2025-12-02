using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StationeryStore.Application.Interfaces;

namespace StationeryStore.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly ConcurrentQueue<string> _entries = new();

    public Task LogAsync(string userId, string action, string? details = null)
    {
        // details can be null from the interface; gracefully handle it
        var safeDetails = details ?? string.Empty;
        var entry = $"{DateTime.UtcNow:O}|{userId}|{action}|{safeDetails}";
        _entries.Enqueue(entry);
        // Keep queue trimmed
        while (_entries.Count > 1000) _entries.TryDequeue(out _);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<string>> GetRecentAsync(int count = 100)
    {
        var items = _entries.ToArray().Reverse().Take(count);
        return Task.FromResult(items.AsEnumerable());
    }
}