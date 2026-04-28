namespace FreelancerDirectory.Domain.Entities;

public class Hobby
{
    public int Id { get; set; }
    public int FreelancerId { get; set; }
    public string HobbyName { get; set; } = string.Empty;
    public Freelancer? Freelancer { get; set; }
}