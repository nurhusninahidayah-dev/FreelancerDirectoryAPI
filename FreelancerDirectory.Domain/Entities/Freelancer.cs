namespace FreelancerDirectory.Domain.Entities;

public class Freelancer
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsArchived { get; set; }
    public List<SkillSet> Skillsets { get; set; } = new();
    public List<Hobby> Hobbies { get; set; } = new();
}