namespace FreelancerDirectory.Domain.Entities;

public class SkillSet
{
    public int Id { get; set; }
    public int FreelancerId { get; set; }
    public string Skill { get; set; } = string.Empty;
    public Freelancer? Freelancer { get; set; }
}