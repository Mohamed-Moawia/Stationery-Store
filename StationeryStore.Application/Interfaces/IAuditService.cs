using System.Collections.Generic;
using System.Threading.Tasks;

namespace StationeryStore.Application.Interfaces;

public interface IAuditService
{
    /// <summary>Log an audit entry (non-blocking).</summary>
    Task LogAsync(string userId, string action, string? details = null);

    /// <summary>Get recent audit entries as simple strings for debugging/inspection.</summary>
    Task<IEnumerable<string>> GetRecentAsync(int count = 100);
}