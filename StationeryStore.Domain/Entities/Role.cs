using StationeryStore.Domain.Common;

namespace StationeryStore.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Optional: navigation to users
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}