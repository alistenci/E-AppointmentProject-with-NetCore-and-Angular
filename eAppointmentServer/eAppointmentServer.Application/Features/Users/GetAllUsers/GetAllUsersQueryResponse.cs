namespace eAppointmentServer.Application.Features.Users.GetAllUsers;

public sealed record GetAllUsersQueryResponse
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public List<Guid> RoleIds { get; set; } = new();
    public List<string?> RoleNames { get; set; } = new();
}
