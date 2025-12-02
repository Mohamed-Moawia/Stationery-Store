using StationeryStore.Domain.Common;

namespace StationeryStore.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}