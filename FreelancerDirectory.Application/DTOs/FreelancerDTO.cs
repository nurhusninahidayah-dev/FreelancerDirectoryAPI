namespace FreelancerDirectory.Application.DTOs;

public class FreelancerDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsArchived { get; set; }
    public List<string> Skillsets { get; set; } = new();
    public List<string> Hobbies { get; set; } = new();
}